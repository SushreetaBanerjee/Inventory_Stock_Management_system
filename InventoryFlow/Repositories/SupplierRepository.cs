using InventoryFlow.Data;
using InventoryFlow.Models;
using InventoryFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Repositories
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context) { }

        public async Task<bool> HasLinkedProductsAsync(int supplierId) =>
            await _context.Products.AnyAsync(p => p.SupplierId == supplierId);
    }
}
