using InventoryFlow.Data;
using InventoryFlow.Models;
using InventoryFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<Product?> GetByIdWithDetailsAsync(int id) =>
            await _dbSet.Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync() =>
            await _dbSet.Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .OrderBy(p => p.Name)
                        .ToListAsync();

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync() =>
            await _dbSet.Include(p => p.Category)
                        .Where(p => p.IsActive && p.CurrentStock <= p.ReorderThreshold)
                        .OrderBy(p => p.CurrentStock)
                        .ToListAsync();

        public async Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null) =>
            await _dbSet.AnyAsync(p => p.SKU == sku && (excludeProductId == null || p.ProductId != excludeProductId));
    }
}
