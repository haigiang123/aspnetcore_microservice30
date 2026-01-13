using Common.Logging;
using Hangfire.API.Extensions;
using Serilog;
using Infrastructure.ScheduledJob;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start Hangfire API up");

try
{
    // Add services to the container.
    builder.Host.AddAppConfigurations();
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddTeduHangfireService();
    builder.Services.ConfigureServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseHangfireDashboard(builder.Configuration);

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", context =>
        {
            // await context.Response.WriteAsync($"Hello TEDU members! This is {builder.Environment.ApplicationName}");
            context.Response.Redirect("swagger/index.html");
            return Task.CompletedTask;
        });
    });

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
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}

