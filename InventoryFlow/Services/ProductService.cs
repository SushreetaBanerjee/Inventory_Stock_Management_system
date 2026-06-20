using InventoryFlow.Models;
using InventoryFlow.Repositories.Interfaces;
using InventoryFlow.Services.Interfaces;

namespace InventoryFlow.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _transactionRepository;

        public ProductService(IProductRepository productRepository, IStockTransactionRepository transactionRepository)
        {
            _productRepository = productRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _productRepository.GetAllWithDetailsAsync();

        public async Task<Product?> GetByIdAsync(int id) => await _productRepository.GetByIdWithDetailsAsync(id);

        public async Task<IEnumerable<Product>> GetLowStockAsync() => await _productRepository.GetLowStockProductsAsync();

        public async Task<(bool Success, string? Error)> CreateAsync(Product product)
        {
            if (await _productRepository.SkuExistsAsync(product.SKU))
                return (false, $"SKU '{product.SKU}' is already in use by another product.");

            // New products always start at zero on-hand stock; stock is only ever
            // added via a Stock-In transaction so the ledger stays the single source of truth.
            product.CurrentStock = 0;
            product.CreatedAt = DateTime.Now;

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.ProductId);
            if (existing == null) return (false, "Product not found.");

            if (await _productRepository.SkuExistsAsync(product.SKU, product.ProductId))
                return (false, $"SKU '{product.SKU}' is already in use by another product.");

            existing.SKU = product.SKU;
            existing.Name = product.Name;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;
            existing.UnitOfMeasure = product.UnitOfMeasure;
            existing.ReorderThreshold = product.ReorderThreshold;
            existing.CostPrice = product.CostPrice;
            existing.SellingPrice = product.SellingPrice;
            existing.IsActive = product.IsActive;
            // Note: CurrentStock is intentionally NOT editable here — it only ever
            // changes via Stock In / Stock Out transactions (see StockService).

            _productRepository.Update(existing);
            await _productRepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return (false, "Product not found.");

            var history = await _transactionRepository.GetHistoryForProductAsync(id);
            if (history.Any())
                return (false, "This product has stock movement history and cannot be deleted. Mark it inactive instead.");

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync();
            return (true, null);
        }
    }
}
