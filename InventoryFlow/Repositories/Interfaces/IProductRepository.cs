using InventoryFlow.Models;

namespace InventoryFlow.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null);
    }
}
