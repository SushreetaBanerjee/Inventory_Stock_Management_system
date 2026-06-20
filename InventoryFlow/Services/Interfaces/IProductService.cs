using InventoryFlow.Models;

namespace InventoryFlow.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetLowStockAsync();
        Task<(bool Success, string? Error)> CreateAsync(Product product);
        Task<(bool Success, string? Error)> UpdateAsync(Product product);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}
