using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryFlow.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(40)]
        [Display(Name = "SKU")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Primary Supplier")]
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; } = "pcs";

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder threshold cannot be negative.")]
        [Display(Name = "Reorder Threshold")]
        public int ReorderThreshold { get; set; }

        /// <summary>
        /// Denormalized / cached running total of stock on hand.
        /// This is the fast-read value used everywhere in the UI (lists, dashboard, badges).
        /// It is ALWAYS updated transactionally alongside a StockTransaction insert
        /// (see StockService), so it should never drift from the ledger under normal use.
        /// It can be fully recalculated from StockTransactions at any time via
        /// IStockService.RecalculateStockAsync() for reconciliation/auditing.
        /// </summary>
        [Display(Name = "Current Stock")]
        public int CurrentStock { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost price cannot be negative.")]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Selling price cannot be negative.")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

        // Convenience / computed (not mapped)
        [NotMapped]
        public bool IsLowStock => CurrentStock <= ReorderThreshold;

        [NotMapped]
        public decimal StockValue => CurrentStock * CostPrice;
    }
}
