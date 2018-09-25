using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TestApp.Data
{
    public class TestAppDbContextFactory : IDesignTimeDbContextFactory<TestAppDbContext>
    {
        public TestAppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TestAppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.CommandTimeout((int) TimeSpan.FromMinutes(10).TotalSeconds));
            return new TestAppDbContext(builder.Options);
        }
    }
}