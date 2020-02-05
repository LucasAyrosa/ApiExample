using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoAPI.Repository.Data;

namespace TodoApiTest.Config
{
    public class TodoApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
                if (descriptor != null) services.Remove(descriptor);
                services.AddDbContext<TodoContext>(options =>
                {
                    options.UseInMemoryDatabase("TodoItemsTest");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TodoContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<TodoApplicationFactory<TStartup>>>();


                    if (db.Database.EnsureCreated())
                    {
                        try
                        {
                            Utilities.InitializeDbForTests(db);
                        }
                        catch (System.Exception)
                        {
                            throw;
                        }
                    }
                }
            });
        }
    }
}