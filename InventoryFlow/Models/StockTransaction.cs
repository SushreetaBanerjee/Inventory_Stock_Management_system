using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryFlow.Models.Enums;

namespace InventoryFlow.Models
{
    /// <summary>
    /// Single unified ledger table for ALL stock movement (in and out), distinguished by
    /// TransactionType. This is the system of record / audit trail. Product.CurrentStock
    /// is a derived cache kept in sync with this table for fast reads.
    ///
    /// Why one table instead of separate StockIn / StockOut tables?
    /// - One chronological, append-only "bank statement" per product — easy to query
    ///   full history with a single ORDER BY, no UNION needed.
    /// - New movement types (adjustments, returns, transfers) can be added later by
    ///   extending the enum, without a schema/table change.
    /// - Auditability: nothing is ever updated or deleted here, only inserted, so the
    ///   ledger is a trustworthy, append-only audit log — exactly like a financial ledger.
    /// - Reporting (stock value over time, movement velocity, supplier reliability) is
    ///   simpler against one table than reconciling two.
    /// </summary>
    public class StockTransaction
    {
        public int TransactionId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Display(Name = "Type")]
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Relevant for StockIn (who supplied it). Null for most StockOut entries.
        /// </summary>
        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Unit cost at time of receipt. Populated for StockIn; null for StockOut.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Cost")]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Only meaningful for StockOut transactions (Sale / Damage / InternalUse / Other).
        /// </summary>
        [Display(Name = "Reason")]
        public StockOutReason? Reason { get; set; }

        [Required]
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Today;

        [StringLength(300)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Snapshot of the running stock balance for this product immediately AFTER
        /// this transaction was applied. Makes the ledger view read like a bank
        /// statement (Balance column) without recomputation at query time.
        /// </summary>
        [Display(Name = "Balance After")]
        public int BalanceAfterTransaction { get; set; }
    }
}
