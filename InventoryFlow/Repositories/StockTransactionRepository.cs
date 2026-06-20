using InventoryFlow.Data;
using InventoryFlow.Models;
using InventoryFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Repositories
{
    public class StockTransactionRepository : Repository<StockTransaction>, IStockTransactionRepository
    {
        public StockTransactionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<StockTransaction>> GetHistoryForProductAsync(int productId) =>
            await _dbSet.Include(t => t.Supplier)
                        .Where(t => t.ProductId == productId)
                        .OrderByDescending(t => t.TransactionDate)
                        .ThenByDescending(t => t.TransactionId)
                        .ToListAsync();

        public async Task<IEnumerable<StockTransaction>> GetRecentAsync(int count) =>
            await _dbSet.Include(t => t.Product)
                        .Include(t => t.Supplier)
                        .OrderByDescending(t => t.TransactionDate)
                        .ThenByDescending(t => t.TransactionId)
                        .Take(count)
                        .ToListAsync();

        public async Task<IEnumerable<StockTransaction>> GetSinceAsync(DateTime sinceDate) =>
            await _dbSet.Include(t => t.Product)
                        .Where(t => t.TransactionDate >= sinceDate)
                        .ToListAsync();
    }
}
