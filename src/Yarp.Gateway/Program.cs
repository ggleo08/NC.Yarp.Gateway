using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Yarp.Gateway.Entities;
using Yarp.Gateway.EntityFrameworkCore;
using Yarp.Gateway.Extensions;
using Yarp.Gateway.Validators;
using Yarp.Gateway.RedisPubSub;
using Yarp.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

#region  动态添加 Yarp 的 dapr-sidercar 访问地址
//builder.WebHost.ConfigureAppConfiguration((context, configBuilder) =>
//{
//    configBuilder.AddDaprConfig();
//});
#endregion

IConfiguration configuration = builder.Configuration;

// Add services to the container.

#region Add Cors、MemoryCache、Controllers、Dapr
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(configuration["App:CorsOrigins"].Split(",", StringSplitOptions.RemoveEmptyEntries))
               .SetIsOriginAllowedToAllowWildcardSubdomains()
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
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

#region Add AddAuthentication、Authorization-YarpCustomPolicy

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

#region Add YARP、LoadFromEntityFramework Service、Redis PubSub
builder.Services.AddReverseProxy()
                // .LoadFromConfig(configuration.GetSection("Yarp"))
                .LoadFromEntityFramework()
                // 加上自定义路由转换
                .AddTransforms<YarpDaprTransformProvider>()
                .AddRedis("10.17.9.30");
#endregion

#region Add Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Add Yarp AppServices、Validator
// 添加 AppServices
builder.Services.AddTransient<IYarpRouteAppService, YarpRouteAppService>();
builder.Services.AddTransient<IYarpClusterAppService, YarpClusterAppService>();

// 添加验证器
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

    // 添加内部服务的Swagger终点 
    app.UseSwaggerUIWithYarp();
    // 访问网关地址，自动跳转到 /swagger 的首页
    app.UseRewriter(new RewriteOptions()
       // Regex for "", "/" and "" (whitespace)
       .AddRedirect("^(|\\|\\s+)$", "/swagger"));
}
#endregion

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// 将云事件中间件添加到中间件管道
app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    // 反向代理路由添加到路由表
    endpoints.MapReverseProxy(proxyPipeline =>
    {
        proxyPipeline.UseLoadBalancing();
    });
    // 将响应请求的端点从 dapr运行时 映射到 /dapr/subscribe
    endpoints.MapSubscribeHandler();
});

app.Run("http://*:5200");
