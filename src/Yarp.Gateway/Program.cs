using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Yarp.Gateway.Entities;
using Yarp.Gateway.EntityFrameworkCore;
using Yarp.Gateway.Extensions;
using Yarp.Gateway.Validators;
using Yarp.Gateway.RedisPubSub;
using Yarp.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

#region  ��̬��� Yarp �� dapr-sidercar ���ʵ�ַ
//builder.WebHost.ConfigureAppConfiguration((context, configBuilder) =>
//{
//    configBuilder.AddDaprConfig();
//});
#endregion

IConfiguration configuration = builder.Configuration;

// Add services to the container.

#region Add Cors��MemoryCache��Controllers��Dapr
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(configuration["App:CorsOrigins"].Split(",", StringSplitOptions.RemoveEmptyEntries))
               .SetIsOriginAllowedToAllowWildcardSubdomains()
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
               // .AllowAnyOrigin();
    });
});

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

builder.Services.AddControllers().AddDapr();

builder.Services.AddMemoryCache();

#endregion

#region Add DbContext -> YarpDbContext
builder.Services.AddDbContext<YarpDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), builder => builder.MigrationsAssembly("Yarp.Gateway"));
    //options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("ReverseProxy.WebApi")));
});
#endregion

#region Add YARP��LoadFromEntityFramework Service��Redis PubSub
builder.Services.AddReverseProxy()
                // .LoadFromConfig(builder.Configuration.GetSection("Yarp"))
                .LoadFromEntityFramework()
                // .AddTransforms<YarpDaprTransformProvider>() // �����Զ���ת��
                .AddRedis("10.17.9.30") // TODO...
                ;
#endregion

#region Add AddAuthentication��Authorization-YarpCustomPolicy

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // builder.Configuration.Bind("JwtSettings", options);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("YarpCustomPolicy", policy => policy.RequireAuthenticatedUser());
});
#endregion

#region Add Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Add Yarp AppServices��Validator
// ��� AppServices
builder.Services.AddTransient<IYarpRouteAppService, YarpRouteAppService>();
builder.Services.AddTransient<IYarpClusterAppService, YarpClusterAppService>();

// �����֤��
builder.Services.AddSingleton<IValidator<YarpCluster>, YarpClusterValidator>();
builder.Services.AddSingleton<IValidator<YarpRoute>, YarpRouteValidator>();
#endregion

#region Add DarClient
builder.Services.AddDaprClient();
#endregion

var app = builder.Build();
app.UseRouting();
app.UseCors();

#region Swagger Redirect
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway v1.0");
    //    options.RoutePrefix = String.Empty;
    //});

    // ����ڲ������Swagger�յ� 
    app.UseSwaggerUIWithYarp();
    // �������ص�ַ���Զ���ת�� /swagger ����ҳ
    app.UseRewriter(new RewriteOptions()
       // Regex for "", "/" and "" (whitespace)
       .AddRedirect("^(|\\|\\s+)$", "/swagger"));
}
#endregion

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.UseLoadBalancing();
});

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
});

app.Run("http://*:5200");
