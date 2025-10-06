using Customer.API.Entities;
using Customer.API.Repositories.Interface;
using Customer.API.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Customer.API
{
    public static class CustomerController
    {
        public static void MapCustomerApi(this WebApplication app)
        {
            app.MapGet("/api/customers", 
                async (ICustomerService customer) =>
                {
                   return await customer.GetCustomersAsync();
                }); 
            app.MapGet("/api/customers/{username}",
                async (string username, ICustomerService customer) => await customer.GetCustomerByUsernameAsync(username));
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
        }
    }
}
