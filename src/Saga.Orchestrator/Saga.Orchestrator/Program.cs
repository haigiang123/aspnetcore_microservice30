using Common.Logging;
using Saga.Orchestrator;
using Saga.Orchestrator.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start Product API up");

try
{
    builder.Host.AddAppConfigurations();
    builder.Host.UseSerilog(Serilogger.Configure);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
    builder.Services.ConfigServices();
    builder.Services.ConfigHttpRepository();
    builder.Services.ConfigHttpClient();
   
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseRouting();
    app.UseAuthorization();
    
    if (builder.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", context =>
            {
                // await context.Response.WriteAsync($"Hello TEDU members! This is {builder.Environment.ApplicationName}");
                context.Response.Redirect("swagger/index.html");
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
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}
