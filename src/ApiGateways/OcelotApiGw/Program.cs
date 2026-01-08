using Common.Logging;
using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start Product API up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddHostExtensions();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddServiceExtentions(builder.Configuration);
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseCors();
    app.UseMiddleware<ErrorWrappingMiddleware>();
    app.UseRouting();

    //app.UseHttpsRedirection();
    //app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", context =>
        {
            // await context.Response.WriteAsync($"Hello TEDU members! This is {builder.Environment.ApplicationName}");
            context.Response.Redirect("swagger/index.html");
            return Task.CompletedTask;
        });
    });

    app.UseSwaggerForOcelotUI(
        opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });
    await app.UseOcelot();

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
