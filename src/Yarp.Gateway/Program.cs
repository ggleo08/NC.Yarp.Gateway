using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Dapr.Client;
using FluentValidation;
using FluentValidation.AspNetCore;
using Yarp.Gateway.Entities;
using Yarp.Gateway.EntityFrameworkCore;
using Yarp.Gateway.Extensions;
using Yarp.Gateway.Validators;
using Yarp.Gateway.RedisPubSub;
using Yarp.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

#region  Dpar ������ã���̬���Yarp��Clusters�Զ�������  -- Disabled
//builder.WebHost.ConfigureAppConfiguration((context, configBuilder) =>
//{
//    var httpEndpoint = DaprDefaults.GetDefaultHttpEndpoint();
//    configBuilder.AddInMemoryCollection(new[]
//    {
//        new KeyValuePair<string, string>("Yarp:Clusters:dapr-sidecar:Destinations:d1:Address", httpEndpoint),
//    });
//});
#endregion

IConfiguration configuration = builder.Configuration;

// Add services to the container.
#region Add YarpDbContext
builder.Services.AddDbContext<YarpDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), builder => builder.MigrationsAssembly("Yarp.Gateway"));
    //options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("ReverseProxy.WebApi")));
});
#endregion

#region Add YARP��LoadFromEntityFramework Middleware
builder.Services.AddReverseProxy()
                // .LoadFromConfig(builder.Configuration.GetSection("Yarp"))
                .LoadFromEntityFramework()
                .AddRedis("10.17.9.30") // TODO...
                ;
#endregion

#region Add Cors��MemoryCache��Controllers
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(configuration["App:CorsOrigins"].Split(",", StringSplitOptions.RemoveEmptyEntries))
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); ;
    });
});

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1.0", new OpenApiInfo
    {
        Title = "Gateway API",
        Version = "v1.0",
        Description = "Gateway API v1.0",

        #region �������� Url��API ��ϵ��Ϣ��API �����Ϣ
        // TermsOfService = new Uri("https://example.com/terms"),

        // API ��ϵ��Ϣ
        //Contact = new OpenApiContact
        //{
        //    Name = "Example Contact",
        //    Url = new Uri("https://example.com/contact")
        //},

        //  API �����Ϣ
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
        #endregion
    });
    // ?
    options.DocInclusionPredicate((docName, description) => true);
    // ?
    options.CustomSchemaIds(type => type.FullName);
});
#endregion

#region ע����������
// ��� AppServices
builder.Services.AddTransient<IYarpRouteAppService, YarpRouteAppService>();
builder.Services.AddTransient<IYarpClusterAppService, YarpClusterAppService>();

// �����֤��
builder.Services.AddSingleton<IValidator<YarpCluster>, YarpClusterValidator>();
builder.Services.AddSingleton<IValidator<YarpRoute>, YarpRouteValidator>();

#endregion


var app = builder.Build();

#region Swagger Redirect
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1.0");
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

app.MapReverseProxy();

app.Run("http://*:5200");
