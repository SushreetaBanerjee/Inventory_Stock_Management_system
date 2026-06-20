using InventoryFlow.Models;

namespace InventoryFlow.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(Supplier supplier);
        Task<(bool Success, string? Error)> UpdateAsync(Supplier supplier);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
