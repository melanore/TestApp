using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestApp.Data.Entities;
using TestApp.Data.Services;

namespace TestApp.Data
{
    public class TestAppDbContext : DbContext
    {
        public TestAppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // EF core by default is not using lazy loading, and that's a desired behavior IMO.
            optionsBuilder.ReplaceService<IEntityMaterializerSource, CustomEntityMaterializerSource>();
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MapCustomers(modelBuilder);
            MapAddresses(modelBuilder);
        }
        
        private static void MapCustomers(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<Customer>();
            entityBuilder.HasKey(s => new { CustomerId = s.Id, s.Name });
            entityBuilder.HasAlternateKey(s => s.Id).HasName("IX_CustomerId");

            // why not just use Identity PK? in case this req is from mapping to existing legacy db, otherwise IMO it's not justified 
            // in most cases auto incremented BIGINT for PK is more then enough, other problems can be solved by enforcing computed columns etc 
            entityBuilder.Property<int>("dbID").IsRequired().UseSqlServerIdentityColumn();
            var propertyBuilder = entityBuilder.Property(s => s.Id)
                .HasColumnName("CustomerId")
                .HasColumnType("varchar(5)")
                .HasMaxLength(5)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("AA000")
                //https://www.mssqltips.com/sqlservertip/1682/using-computed-columns-in-sql-server-with-persisted-values/
                //https://www.sqlteam.com/articles/custom-auto-generated-sequences-with-sql-server
                //pre-calculated, correctly typed and persisted to satisfy sql server
                .HasComputedColumnSql("CAST(ISNULL([dbo].[CustomerNumber](dbID), 'AA000') AS varchar(5)) PERSISTED");
            
            //https://github.com/aspnet/EntityFrameworkCore/issues/7653
            propertyBuilder.Metadata.AfterSaveBehavior = PropertySaveBehavior.Throw;
            propertyBuilder.Metadata.BeforeSaveBehavior = PropertySaveBehavior.Throw;

            entityBuilder.Property(s => s.Name).HasColumnType("nvarchar(100)").HasMaxLength(100);
            entityBuilder.Property(s => s.Street).HasColumnType("nvarchar(100)").HasMaxLength(100);
            entityBuilder.Property(s => s.Zip).HasColumnType("varchar(20)").HasColumnName("ZIP").HasMaxLength(20);
            entityBuilder.Property(s => s.City).HasColumnType("nvarchar(100)").HasMaxLength(100);
            entityBuilder.Property(s => s.Country).HasColumnType("varchar(2)").HasMaxLength(2);
        }
        
        private static void MapAddresses(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<Address>();
            entityBuilder.HasKey(s => new { s.CustomerId, s.AddressType });
            
            entityBuilder.HasDiscriminator(s => s.AddressType)
                .HasValue<InvoiceAddress>("I")
                .HasValue<DeliveryAddress>("D")
                .HasValue<ServiceAddress>("S");
            
            entityBuilder.Property(s => s.CustomerId).HasColumnType("varchar(5)").HasMaxLength(5);
            entityBuilder.Property(s => s.AddressType).HasColumnType("varchar(1)").HasMaxLength(1);
            
            entityBuilder.Property(s => s.Name).HasColumnType("nvarchar(100)").HasMaxLength(100);
            entityBuilder.Property(s => s.Street).HasColumnType("nvarchar(100)").HasMaxLength(100);
            entityBuilder.Property(s => s.Zip).HasColumnType("varchar(20)").HasColumnName("ZIP").HasMaxLength(20);
            entityBuilder.Property(s => s.City).HasColumnType("nvarchar(100)").HasMaxLength(100);
            entityBuilder.Property(s => s.Country).HasColumnType("varchar(2)").HasMaxLength(2);
            
            entityBuilder.HasOne(s => s.Customer).WithMany(s => s.Addresses).HasForeignKey(s => s.CustomerId).HasPrincipalKey(s => s.Id);
        }
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            PurgeEfTempIdFromInsertStatement();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void PurgeEfTempIdFromInsertStatement()
        {
            var udfBasedComputedColumns = ChangeTracker.Entries().Where(s => s.Entity is Customer && s.State == EntityState.Added)
                .Select(customerEntry => customerEntry.Properties.FirstOrDefault(s => s.Metadata.Name == nameof(Customer.Id)));

            foreach (var propertyEntry in udfBasedComputedColumns.Where(s => s != null)) propertyEntry.IsTemporary = true;
        }
    }
}