using InventoryFlow.Models;
using InventoryFlow.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Supplier ----------
            modelBuilder.Entity<Supplier>(e =>
            {
                e.HasIndex(s => s.Name);
            });

            // ---------- Category ----------
            modelBuilder.Entity<Category>(e =>
            {
                e.HasIndex(c => c.Name).IsUnique();
            });

            // ---------- Product ----------
            modelBuilder.Entity<Product>(e =>
            {
                e.HasIndex(p => p.SKU).IsUnique();

                e.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.ToTable(t => t.HasCheckConstraint("CK_Product_CurrentStock_NonNegative", "[CurrentStock] >= 0"));
                e.ToTable(t => t.HasCheckConstraint("CK_Product_ReorderThreshold_NonNegative", "[ReorderThreshold] >= 0"));
            });

            // ---------- StockTransaction ----------
            modelBuilder.Entity<StockTransaction>(e =>
            {
                e.HasKey(t => t.TransactionId);

                e.HasOne(t => t.Product)
                    .WithMany(p => p.StockTransactions)
                    .HasForeignKey(t => t.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(t => t.Supplier)
                    .WithMany(s => s.StockTransactions)
                    .HasForeignKey(t => t.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(20);
                e.Property(t => t.Reason).HasConversion<string>().HasMaxLength(20);

                e.HasIndex(t => new { t.ProductId, t.TransactionDate });

                e.ToTable(t => t.HasCheckConstraint("CK_StockTransaction_Quantity_Positive", "[Quantity] > 0"));
            });
        }
    }
}
