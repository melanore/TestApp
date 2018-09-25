using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestApp.Data.Services;

namespace TestApp.Data
{
    public class TestAppDbContext : DbContext
    {
        public TestAppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IEntityMaterializerSource, CustomEntityMaterializerSource>();
        }
    }
}