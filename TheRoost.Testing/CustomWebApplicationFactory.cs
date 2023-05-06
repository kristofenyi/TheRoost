using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using TheRoost.API.AppDbContext;
using TheRoost.API.Services;

namespace TheRoost.Testing
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private SqliteConnection _connection;

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var _connection = new SqliteConnection("Datasource=:memory:");
            _connection.Open();

            builder.ConfigureServices(services =>
            {
                var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<MainDbContext>));
                services.Remove(serviceDescriptor);
                services.AddDbContext<MainDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
                services.AddScoped<IEmailService, EmailServiceMock>();
            });

            return base.CreateHost(builder);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
