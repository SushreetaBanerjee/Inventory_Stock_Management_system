using InventoryFlow.Models;
using InventoryFlow.Models.Enums;
using InventoryFlow.Models.ViewModels;

namespace InventoryFlow.Services.Interfaces
{
    public interface IStockService
    {
        Task<(bool Success, string? Error)> StockInAsync(StockInViewModel model);
        Task<(bool Success, string? Error)> StockOutAsync(StockOutViewModel model);
        Task<IEnumerable<StockTransaction>> GetHistoryForProductAsync(int productId);
        Task<DashboardViewModel> GetDashboardDataAsync();

        /// <summary>
        /// Recomputes every product's CurrentStock purely from the StockTransaction
        /// ledger (sum of StockIn minus sum of StockOut). Used as a reconciliation /
        /// integrity-check tool — the cached column should already match this.
        /// </summary>
        Task RecalculateStockAsync();
    }
}
