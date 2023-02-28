// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedWarehousingCore.DAL;
using SharedWarehousingCore.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddSingleton(AutoMapperConfig.Initialize());
builder.Services.AddIdentityServices(configuration);
builder.Services.AddDIContainer();

builder.Services.AddDbContext<MainDbContext>(x =>
    x.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Host.UseSerilog();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = environment == Environments.Development;
var isProduction = environment == Environments.Production;

if (isDevelopment)
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("Logs/log.log",
            rollingInterval: RollingInterval.Day,
            fileSizeLimitBytes: 1000000000,
            retainedFileCountLimit: 31)
        .MinimumLevel.Information()
        .CreateLogger();
}

else
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("Logs/log.log",
            rollingInterval: RollingInterval.Day,
            fileSizeLimitBytes: 1000000000,
            retainedFileCountLimit: 31)
        .MinimumLevel.Error()
        .CreateLogger();
}

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

try
{
    if (isDevelopment)
    {
        Log.Warning("By the power of greyskull... App started");
        Log.Warning(@" ");
    }

    else
    {
        Log.Error("By the power of greyskull... MES started");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}