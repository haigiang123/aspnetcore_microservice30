var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecksUI().AddInMemoryStorage();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecksUI();
    endpoints.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
