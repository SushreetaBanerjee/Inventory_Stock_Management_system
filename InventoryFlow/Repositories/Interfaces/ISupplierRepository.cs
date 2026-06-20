using InventoryFlow.Models;

namespace InventoryFlow.Repositories.Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<bool> HasLinkedProductsAsync(int supplierId);
    }
}
