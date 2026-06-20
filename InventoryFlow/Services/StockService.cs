using InventoryFlow.Data;
using InventoryFlow.Models;
using InventoryFlow.Models.Enums;
using InventoryFlow.Models.ViewModels;
using InventoryFlow.Repositories.Interfaces;
using InventoryFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Services
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _context; // used for an explicit transaction boundary
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _transactionRepository;
        private readonly ISupplierRepository _supplierRepository;

        public StockService(
            AppDbContext context,
            IProductRepository productRepository,
            IStockTransactionRepository transactionRepository,
            ISupplierRepository supplierRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _transactionRepository = transactionRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<(bool Success, string? Error)> StockInAsync(StockInViewModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null) return (false, "Product not found.");
            if (model.Quantity <= 0) return (false, "Quantity must be greater than zero.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                product.CurrentStock += model.Quantity;

                var entry = new StockTransaction
                {
                    ProductId = product.ProductId,
                    TransactionType = TransactionType.StockIn,
                    SupplierId = model.SupplierId,
                    Quantity = model.Quantity,
                    UnitCost = model.UnitCost,
                    TransactionDate = model.TransactionDate,
                    Notes = model.Notes,
                    BalanceAfterTransaction = product.CurrentStock
                };

                _productRepository.Update(product);
                await _transactionRepository.AddAsync(entry);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Something went wrong while recording the stock-in entry. Please try again.");
            }
        }

        public async Task<(bool Success, string? Error)> StockOutAsync(StockOutViewModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null) return (false, "Product not found.");
            if (model.Quantity <= 0) return (false, "Quantity must be greater than zero.");

            // Core business rule: never allow stock to go negative.
            if (model.Quantity > product.CurrentStock)
                return (false, $"Cannot remove {model.Quantity} unit(s) — only {product.CurrentStock} currently in stock.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                product.CurrentStock -= model.Quantity;

                var entry = new StockTransaction
                {
                    ProductId = product.ProductId,
                    TransactionType = TransactionType.StockOut,
                    Quantity = model.Quantity,
                    Reason = model.Reason,
                    TransactionDate = model.TransactionDate,
                    Notes = model.Notes,
                    BalanceAfterTransaction = product.CurrentStock
                };

                _productRepository.Update(product);
                await _transactionRepository.AddAsync(entry);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Something went wrong while recording the stock-out entry. Please try again.");
            }
        }

        public async Task<IEnumerable<StockTransaction>> GetHistoryForProductAsync(int productId) =>
            await _transactionRepository.GetHistoryForProductAsync(productId);

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var products = (await _productRepository.GetAllWithDetailsAsync()).Where(p => p.IsActive).ToList();
            var lowStock = products.Where(p => p.IsLowStock).OrderBy(p => p.CurrentStock).ToList();
            var recent = await _transactionRepository.GetRecentAsync(10);
            var suppliers = await _supplierRepository.GetAllAsync();

            var categoryGroups = products
                .GroupBy(p => p.Category?.Name ?? "Uncategorized")
                .Select(g => new { Category = g.Key, Value = g.Sum(p => p.StockValue) })
                .OrderByDescending(g => g.Value)
                .ToList();

            var since = DateTime.Today.AddDays(-30);
            var recentMoves = await _transactionRepository.GetSinceAsync(since);
            var topMoving = recentMoves
                .GroupBy(t => t.ProductId)
                .Select(g => new
                {
                    Name = g.First().Product?.Name ?? products.FirstOrDefault(p => p.ProductId == g.Key)?.Name ?? "Unknown",
                    Quantity = g.Sum(t => t.Quantity)
                })
                .OrderByDescending(g => g.Quantity)
                .Take(6)
                .ToList();

            return new DashboardViewModel
            {
                TotalStockValue = products.Sum(p => p.StockValue),
                TotalProducts = products.Count,
                LowStockCount = lowStock.Count,
                TotalSuppliers = suppliers.Count(s => s.IsActive),
                LowStockProducts = lowStock,
                RecentTransactions = recent.ToList(),
                CategoryLabels = categoryGroups.Select(g => g.Category).ToList(),
                CategoryStockValues = categoryGroups.Select(g => g.Value).ToList(),
                TopMovingProductNames = topMoving.Select(t => t.Name).ToList(),
                TopMovingProductQuantities = topMoving.Select(t => t.Quantity).ToList()
            };
        }

        public async Task RecalculateStockAsync()
        {
            var products = await _context.Products.ToListAsync();
            foreach (var product in products)
            {
                var net = await _context.StockTransactions
                    .Where(t => t.ProductId == product.ProductId)
                    .SumAsync(t => t.TransactionType == TransactionType.StockIn ? t.Quantity : -t.Quantity);

                product.CurrentStock = Math.Max(0, net);
            }
            await _context.SaveChangesAsync();
        }
    }
}
