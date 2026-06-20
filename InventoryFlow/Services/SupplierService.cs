using InventoryFlow.Models;
using InventoryFlow.Repositories.Interfaces;
using InventoryFlow.Services.Interfaces;

namespace InventoryFlow.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync() =>
            (await _supplierRepository.GetAllAsync()).OrderBy(s => s.Name);

        public async Task<Supplier?> GetByIdAsync(int id) => await _supplierRepository.GetByIdAsync(id);

        public async Task<(bool Success, string? Error)> CreateAsync(Supplier supplier)
        {
            supplier.CreatedAt = DateTime.Now;
            await _supplierRepository.AddAsync(supplier);
            await _supplierRepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Supplier supplier)
        {
            var existing = await _supplierRepository.GetByIdAsync(supplier.SupplierId);
            if (existing == null) return (false, "Supplier not found.");

            existing.Name = supplier.Name;
            existing.ContactPerson = supplier.ContactPerson;
            existing.Phone = supplier.Phone;
            existing.Email = supplier.Email;
            existing.Address = supplier.Address;
            existing.ProductsSupplied = supplier.ProductsSupplied;
            existing.IsActive = supplier.IsActive;

            _supplierRepository.Update(existing);
            await _supplierRepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return (false, "Supplier not found.");

            if (await _supplierRepository.HasLinkedProductsAsync(id))
                return (false, "This supplier is linked to one or more products and cannot be deleted. Mark it inactive instead.");

            _supplierRepository.Remove(supplier);
            await _supplierRepository.SaveChangesAsync();
            return (true, null);
        }
    }

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllAsync() =>
            (await _categoryRepository.GetAllAsync()).OrderBy(c => c.Name);
    }
}
