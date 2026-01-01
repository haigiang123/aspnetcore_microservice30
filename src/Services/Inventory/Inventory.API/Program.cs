using Common.Logging;
using Serilog;
using Inventory.Product.API.Extensions;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start Product API up");

try
{
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastrutureServices();
    builder.Services.ConfigureMonggoDbClient();
    builder.Services.AddSwaggerGen();
    var app = builder.Build();

    // Configure the HTTP request pipeline.

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.MigrateDatabase();
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
