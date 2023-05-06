using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using TheRoost.API;
using TheRoost.API.AppDbContext;

namespace TheRoost.Testing
{
    public class TestingContextProvider
    {
        public static MainDbContext CreateContextFromScratch()
        {
            var dbConnection = new SqliteConnection("Datasource=:memory:");
            dbConnection.Open();
            var contextOptions = new DbContextOptionsBuilder<MainDbContext>().UseSqlite(dbConnection).Options;
            var context = new MainDbContext(contextOptions);
            context.Database.EnsureCreated();
            return context;
        }

        public static MainDbContext CreateContextFromFactory(CustomWebApplicationFactory<Program> factory)
        {
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var connection = context.Database.GetDbConnection();
            connection.Close();
            connection.Open();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}
