using System;
using Microsoft.EntityFrameworkCore;

namespace TestApp.Data
{
    public static class Program
    {
        public static void Main()
        {
            using (var context = new TestAppDbContextFactory().CreateDbContext(Array.Empty<string>()))
            {
                context.Database.SetCommandTimeout((int) TimeSpan.FromMinutes(10).TotalSeconds);
                context.Database.Migrate();
            }
        }
    }
}