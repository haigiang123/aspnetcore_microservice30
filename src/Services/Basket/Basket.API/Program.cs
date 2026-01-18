using Basket.API.Extensions;
using Common.Logging;
using Infrastructure.Extensions;
using Serilog;
using Basket.API;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Start Basket API up");

try
{
    builder.Host.AddAppConfigurations();

    // Add services to the container.
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.ConfigureServices();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureMassTransitWithRabbitMq();
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureHttpClientService();

    var app = builder.Build();

    app.UseRouting();
    if (builder.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", context =>
            {
                // await context.Response.WriteAsync($"Hello TEDU members! This is {builder.Environment.ApplicationName}");
                context.Response.Redirect("/swagger/index.html");
                return Task.CompletedTask;
            });
        });
    }

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Basket API complete");
    Log.CloseAndFlush();
}

