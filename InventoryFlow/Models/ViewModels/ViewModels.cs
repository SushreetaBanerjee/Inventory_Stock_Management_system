using System.ComponentModel.DataAnnotations;
using InventoryFlow.Models.Enums;

namespace InventoryFlow.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalStockValue { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int TotalSuppliers { get; set; }
        public List<Product> LowStockProducts { get; set; } = new();
        public List<StockTransaction> RecentTransactions { get; set; } = new();

        // Chart data: stock value grouped by category
        public List<string> CategoryLabels { get; set; } = new();
        public List<decimal> CategoryStockValues { get; set; } = new();

        // Chart data: top moving products (by total quantity moved, last 30 days)
        public List<string> TopMovingProductNames { get; set; } = new();
        public List<int> TopMovingProductQuantities { get; set; } = new();
    }

    public class StockInViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Product")]
        public List<Product> Products { get; set; } = new();

        [Required(ErrorMessage = "Please select a supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        public List<Supplier> Suppliers { get; set; } = new();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0.")]
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; } = DateTime.Today;

        [StringLength(300)]
        public string? Notes { get; set; }
    }

    public class StockOutViewModel
    {
        public int ProductId { get; set; }
        public List<Product> Products { get; set; } = new();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please select a reason.")]
        public StockOutReason Reason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; } = DateTime.Today;

        [StringLength(300)]
        public string? Notes { get; set; }

        // Used to show available stock on the form / re-render on validation failure
        public int AvailableStock { get; set; }
    }

    public class ProductFormViewModel
    {
        public Product Product { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
    }
}
