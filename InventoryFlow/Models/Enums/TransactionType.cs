namespace InventoryFlow.Models.Enums
{
    /// <summary>
    /// Distinguishes a stock-in (purchase/receipt) from a stock-out (sale/issue) entry
    /// within the single unified StockTransaction ledger table.
    /// </summary>
    public enum TransactionType
    {
        StockIn = 1,
        StockOut = 2
    }

    /// <summary>
    /// Reason captured for a Stock-Out transaction. Only relevant when
    /// TransactionType == StockOut; nullable on StockIn rows.
    /// </summary>
    public enum StockOutReason
    {
        Sale = 1,
        Damage = 2,
        InternalUse = 3,
        Other = 4
    }
}
