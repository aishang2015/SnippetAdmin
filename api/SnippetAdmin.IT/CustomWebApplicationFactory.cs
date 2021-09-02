using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.RBAC;
using System.Linq;

namespace SnippetAdmin.IT
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                  d => d.ServiceType ==
                      typeof(DbContextOptions<SnippetAdminDbContext>));

                services.Remove(descriptor);

                services.AddDbContext<SnippetAdminDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<SnippetAdminDbContext>();
                    db.Database.EnsureCreated();

                    var userManager = scopedServices.GetRequiredService<UserManager<SnippetAdminUser>>();
                    userManager.CreateAsync(new SnippetAdminUser
                    {
                        UserName = "admin",
                        Email = "admin@tttttttttt.com.cn",
                        PhoneNumber = "16655558888",
                    }, "admin2").Wait();
                }
            });
        }
    }
}