using Ordering.Infrastructure;
using Common.Logging;
using Serilog;
using Ordering.Infrastructure.Persistence;
using Ordering.Application;
using Ordering.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
Log.Information("Start Order API up");
try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations(builder.Configuration);

    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastructureService(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();
    builder.Services.AddApplicationService();
    builder.Services.ConfigureMassTransit();
    builder.Services.AddEndpointsApiExplorer();
    var app = builder.Build();

    // Configure the HTTP request pipeline.

    //app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllers();
    app.MapDefaultControllerRoute();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
        await orderContextSeed.InitialiseAsync();
        await orderContextSeed.SeedAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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
    Log.Information("Shut down Order API complete");
    Log.CloseAndFlush();
}
