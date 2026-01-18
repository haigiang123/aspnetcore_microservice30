using Serilog;
using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interface;
using Customer.API.Service;
using Customer.API.Service.Interface;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Customer.API;
using Customer.API.Extensions;
using Infrastructure.ScheduledJob;
using Shared.Configurations;
using Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Start Customer API up");

try
{
    builder.Host.AddAppConfigurations();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddTeduHangfireService();

    var databaseSettings = builder.Configuration.GetSection(nameof(DatabaseSettings))
        .Get<DatabaseSettings>();
    if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
        throw new ArgumentNullException("Connection string is not configured.");
    
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(databaseSettings.ConnectionString));

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
                    .AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryQueryBase<,,>))
                    .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                    .AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();

    //// Syntax minimal api 
    app.MapCustomerApi();
    ///we can use in Program.cs or controller
    //app.MapGet("/api/customers",
    //    async (ICustomerService customer) => await customer.GetCustomersAsync());
    //app.MapGet("/api/customers/{username}",
    //    async (string username, ICustomerService customer) => await customer.GetCustomerByUsernameAsync(username));
    //app.MapPost("", async (Customer.API.Entities.Customer customer, ICustomerRepository repository) =>
    //{
    //    await repository.CreateAsync(customer);
    //    await repository.SaveChangesAsync();
    //});
    //app.MapDelete("/api/customers/{userId}",
    //    async (int userId, ICustomerRepository customer) =>
    //    {
    //        var user = await customer.FindByCondition(x => x.Id.Equals(userId)).SingleOrDefaultAsync();

    //        if (user == null)
    //        {
    //            return Results.NotFound();
    //        }

    //        await customer.DeleteAsync(user);
    //        await customer.SaveChangesAsync();

    //        return Results.NoContent();
    //    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ErrorWrappingMiddleware>();

    app.UseAuthorization();

    app.UseHangfireDashboard(builder.Configuration);

    app.MapControllers();

    //app.SeedCustomerData().Run();
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

