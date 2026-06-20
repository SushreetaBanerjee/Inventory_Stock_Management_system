using InventoryFlow.Models;

namespace InventoryFlow.Repositories.Interfaces
{
    public interface IStockTransactionRepository : IRepository<StockTransaction>
    {
        Task<IEnumerable<StockTransaction>> GetHistoryForProductAsync(int productId);
        Task<IEnumerable<StockTransaction>> GetRecentAsync(int count);
        Task<IEnumerable<StockTransaction>> GetSinceAsync(DateTime sinceDate);
    }
}
