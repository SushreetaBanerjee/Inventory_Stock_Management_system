using InventoryFlow.Models;
using InventoryFlow.Models.Enums;

namespace InventoryFlow.Data
{
    /// <summary>
    /// Generates realistic demo data so the dashboard, low-stock alerts, and
    /// movement ledger look populated for screenshots / a demo video.
    /// Runs once on startup if the database has no products yet (see Program.cs).
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Assumes context.Database.Migrate() has already been called (see Program.cs)
            if (context.Products.Any()) return; // already seeded

            // ---------------- Categories ----------------
            var categories = new List<Category>
            {
                new() { Name = "Electronics", Description = "Consumer electronics and accessories" },
                new() { Name = "Stationery", Description = "Office and school supplies" },
                new() { Name = "Groceries", Description = "Packaged food and household consumables" },
                new() { Name = "Furniture", Description = "Office and home furniture" },
                new() { Name = "Cleaning Supplies", Description = "Cleaning and sanitation products" },
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            int catElectronics = categories[0].CategoryId;
            int catStationery = categories[1].CategoryId;
            int catGroceries = categories[2].CategoryId;
            int catFurniture = categories[3].CategoryId;
            int catCleaning = categories[4].CategoryId;

            // ---------------- Suppliers ----------------
            var suppliers = new List<Supplier>
            {
                new() { Name = "BrightTech Distributors", ContactPerson = "Arjun Mehta", Phone = "9876501234", Email = "sales@brighttech.example", Address = "Sector 18, Noida, UP", ProductsSupplied = "Electronics, Cables" },
                new() { Name = "PaperCraft Wholesale", ContactPerson = "Neha Sharma", Phone = "9123456780", Email = "orders@papercraft.example", Address = "MG Road, Pune, MH", ProductsSupplied = "Stationery" },
                new() { Name = "Daily Basket Suppliers", ContactPerson = "Rohit Verma", Phone = "9988776655", Email = "rohit@dailybasket.example", Address = "Salt Lake, Kolkata, WB", ProductsSupplied = "Groceries" },
                new() { Name = "UrbanSpace Furnishings", ContactPerson = "Kavita Iyer", Phone = "9090909090", Email = "kavita@urbanspace.example", Address = "Whitefield, Bengaluru, KA", ProductsSupplied = "Furniture" },
                new() { Name = "ShineWell Chemicals", ContactPerson = "Manoj Yadav", Phone = "9001122334", Email = "manoj@shinewell.example", Address = "Andheri, Mumbai, MH", ProductsSupplied = "Cleaning Supplies" },
                new() { Name = "GadgetHub Imports", ContactPerson = "Priya Nair", Phone = "9876123450", Email = "priya@gadgethub.example", Address = "T Nagar, Chennai, TN", ProductsSupplied = "Electronics" },
                new() { Name = "OfficeMax Traders", ContactPerson = "Sandeep Roy", Phone = "9112233445", Email = "sandeep@officemax.example", Address = "Park Street, Kolkata, WB", ProductsSupplied = "Stationery, Furniture" },
                new() { Name = "FreshFarm Foods", ContactPerson = "Anita Das", Phone = "9871234560", Email = "anita@freshfarm.example", Address = "Sector 62, Noida, UP", ProductsSupplied = "Groceries" },
                new() { Name = "CleanCo Supplies", ContactPerson = "Vikram Singh", Phone = "9000099999", Email = "vikram@cleanco.example", Address = "Hinjewadi, Pune, MH", ProductsSupplied = "Cleaning Supplies" },
            };
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            int sBrightTech = suppliers[0].SupplierId;
            int sPaperCraft = suppliers[1].SupplierId;
            int sDailyBasket = suppliers[2].SupplierId;
            int sUrbanSpace = suppliers[3].SupplierId;
            int sShineWell = suppliers[4].SupplierId;
            int sGadgetHub = suppliers[5].SupplierId;
            int sOfficeMax = suppliers[6].SupplierId;
            int sFreshFarm = suppliers[7].SupplierId;
            int sCleanCo = suppliers[8].SupplierId;

            // ---------------- Products ----------------
            // CurrentStock and CreatedAt are placeholders here; they get finalized
            // by the transaction simulation below.
            var products = new List<Product>
            {
                // Electronics
                new() { SKU = "ELE-001", Name = "Wireless Mouse", CategoryId = catElectronics, SupplierId = sBrightTech, UnitOfMeasure = "pcs", ReorderThreshold = 15, CostPrice = 350, SellingPrice = 599 },
                new() { SKU = "ELE-002", Name = "USB-C Cable 1m", CategoryId = catElectronics, SupplierId = sBrightTech, UnitOfMeasure = "pcs", ReorderThreshold = 30, CostPrice = 80, SellingPrice = 199 },
                new() { SKU = "ELE-003", Name = "Bluetooth Headset", CategoryId = catElectronics, SupplierId = sGadgetHub, UnitOfMeasure = "pcs", ReorderThreshold = 10, CostPrice = 900, SellingPrice = 1499 },
                new() { SKU = "ELE-004", Name = "65W Power Bank", CategoryId = catElectronics, SupplierId = sGadgetHub, UnitOfMeasure = "pcs", ReorderThreshold = 12, CostPrice = 1100, SellingPrice = 1799 },
                new() { SKU = "ELE-005", Name = "HDMI Cable 2m", CategoryId = catElectronics, SupplierId = sBrightTech, UnitOfMeasure = "pcs", ReorderThreshold = 20, CostPrice = 150, SellingPrice = 349 },
                new() { SKU = "ELE-006", Name = "Laptop Cooling Pad", CategoryId = catElectronics, SupplierId = sGadgetHub, UnitOfMeasure = "pcs", ReorderThreshold = 8, CostPrice = 650, SellingPrice = 999 },

                // Stationery
                new() { SKU = "STA-001", Name = "A4 Paper Ream (500 sheets)", CategoryId = catStationery, SupplierId = sPaperCraft, UnitOfMeasure = "ream", ReorderThreshold = 25, CostPrice = 220, SellingPrice = 320 },
                new() { SKU = "STA-002", Name = "Gel Pen (Blue) Box of 10", CategoryId = catStationery, SupplierId = sPaperCraft, UnitOfMeasure = "box", ReorderThreshold = 20, CostPrice = 60, SellingPrice = 110 },
                new() { SKU = "STA-003", Name = "Spiral Notebook A5", CategoryId = catStationery, SupplierId = sOfficeMax, UnitOfMeasure = "pcs", ReorderThreshold = 30, CostPrice = 35, SellingPrice = 65 },
                new() { SKU = "STA-004", Name = "Stapler Heavy Duty", CategoryId = catStationery, SupplierId = sOfficeMax, UnitOfMeasure = "pcs", ReorderThreshold = 10, CostPrice = 120, SellingPrice = 220 },
                new() { SKU = "STA-005", Name = "Sticky Notes Pack", CategoryId = catStationery, SupplierId = sPaperCraft, UnitOfMeasure = "pack", ReorderThreshold = 25, CostPrice = 45, SellingPrice = 85 },

                // Groceries
                new() { SKU = "GRO-001", Name = "Basmati Rice 5kg", CategoryId = catGroceries, SupplierId = sDailyBasket, UnitOfMeasure = "bag", ReorderThreshold = 20, CostPrice = 420, SellingPrice = 549 },
                new() { SKU = "GRO-002", Name = "Sunflower Oil 1L", CategoryId = catGroceries, SupplierId = sFreshFarm, UnitOfMeasure = "bottle", ReorderThreshold = 30, CostPrice = 140, SellingPrice = 189 },
                new() { SKU = "GRO-003", Name = "Toor Dal 1kg", CategoryId = catGroceries, SupplierId = sDailyBasket, UnitOfMeasure = "pack", ReorderThreshold = 25, CostPrice = 110, SellingPrice = 149 },
                new() { SKU = "GRO-004", Name = "Green Tea 100 Bags", CategoryId = catGroceries, SupplierId = sFreshFarm, UnitOfMeasure = "box", ReorderThreshold = 15, CostPrice = 180, SellingPrice = 249 },
                new() { SKU = "GRO-005", Name = "Biscuits Family Pack", CategoryId = catGroceries, SupplierId = sDailyBasket, UnitOfMeasure = "pack", ReorderThreshold = 40, CostPrice = 60, SellingPrice = 95 },

                // Furniture
                new() { SKU = "FUR-001", Name = "Ergonomic Office Chair", CategoryId = catFurniture, SupplierId = sUrbanSpace, UnitOfMeasure = "pcs", ReorderThreshold = 4, CostPrice = 4200, SellingPrice = 6499 },
                new() { SKU = "FUR-002", Name = "Study Table 3ft", CategoryId = catFurniture, SupplierId = sUrbanSpace, UnitOfMeasure = "pcs", ReorderThreshold = 5, CostPrice = 3100, SellingPrice = 4799 },
                new() { SKU = "FUR-003", Name = "Bookshelf 5-Tier", CategoryId = catFurniture, SupplierId = sOfficeMax, UnitOfMeasure = "pcs", ReorderThreshold = 4, CostPrice = 2600, SellingPrice = 3999 },

                // Cleaning Supplies
                new() { SKU = "CLN-001", Name = "Floor Cleaner 1L", CategoryId = catCleaning, SupplierId = sShineWell, UnitOfMeasure = "bottle", ReorderThreshold = 25, CostPrice = 90, SellingPrice = 145 },
                new() { SKU = "CLN-002", Name = "Hand Sanitizer 500ml", CategoryId = catCleaning, SupplierId = sCleanCo, UnitOfMeasure = "bottle", ReorderThreshold = 30, CostPrice = 75, SellingPrice = 129 },
                new() { SKU = "CLN-003", Name = "Dishwash Liquid 500ml", CategoryId = catCleaning, SupplierId = sShineWell, UnitOfMeasure = "bottle", ReorderThreshold = 20, CostPrice = 65, SellingPrice = 99 },
                new() { SKU = "CLN-004", Name = "Trash Bags Roll (30 pcs)", CategoryId = catCleaning, SupplierId = sCleanCo, UnitOfMeasure = "roll", ReorderThreshold = 20, CostPrice = 55, SellingPrice = 89 },
            };

            foreach (var p in products) { p.CurrentStock = 0; p.CreatedAt = DateTime.Now.AddMonths(-4); }
            context.Products.AddRange(products);
            context.SaveChanges();

            // ---------------- Stock Transactions (ledger) ----------------
            // Each product gets an initial stock-in ~4 months ago, then a handful of
            // stock-out (and occasionally a restock) entries spread across the following
            // weeks, ending with realistic current balances - some intentionally below
            // their reorder threshold so the Low-Stock dashboard has data to show.
            var rng = new Random(42); // fixed seed => reproducible demo data
            var transactions = new List<StockTransaction>();
            var today = DateTime.Today;

            void AddTransaction(Product product, TransactionType type, int qty, DateTime date,
                int? supplierId = null, decimal? unitCost = null, StockOutReason? reason = null, string? notes = null)
            {
                if (type == TransactionType.StockOut)
                    qty = Math.Min(qty, product.CurrentStock); // never let demo data go negative
                if (qty <= 0) return;

                if (type == TransactionType.StockIn)
                    product.CurrentStock += qty;
                else
                    product.CurrentStock -= qty;

                transactions.Add(new StockTransaction
                {
                    ProductId = product.ProductId,
                    TransactionType = type,
                    SupplierId = type == TransactionType.StockIn ? supplierId : null,
                    Quantity = qty,
                    UnitCost = type == TransactionType.StockIn ? unitCost : null,
                    Reason = type == TransactionType.StockOut ? reason : null,
                    TransactionDate = date,
                    Notes = notes,
                    CreatedAt = date,
                    BalanceAfterTransaction = product.CurrentStock
                });
            }

            // Plan: every product gets an initial receipt sized a bit above its reorder
            // threshold, then 1-4 sale/issue entries, and about a third of products get
            // a mid-period restock. A few products are deliberately drawn down close to
            // or below threshold to populate the low-stock alert list.
            var lowStockTargets = new HashSet<string> { "ELE-001", "ELE-003", "STA-004", "FUR-001", "CLN-002", "GRO-004" };

            foreach (var p in products)
            {
                int initialQty = p.ReorderThreshold * (lowStockTargets.Contains(p.SKU) ? 2 : 4) + rng.Next(5, 20);
                var initialDate = today.AddDays(-rng.Next(100, 125));
                AddTransaction(p, TransactionType.StockIn, initialQty, initialDate, p.SupplierId, p.CostPrice, notes: "Initial stock receipt");

                int outEvents = rng.Next(2, 5);
                for (int i = 0; i < outEvents; i++)
                {
                    var date = initialDate.AddDays(rng.Next(5, 100));
                    if (date > today) date = today.AddDays(-rng.Next(0, 10));
                    int qty = Math.Max(1, p.CurrentStock / rng.Next(3, 7));
                    var reason = rng.Next(0, 10) < 8 ? StockOutReason.Sale
                               : rng.Next(0, 2) == 0 ? StockOutReason.Damage
                               : StockOutReason.InternalUse;
                    AddTransaction(p, TransactionType.StockOut, qty, date, reason: reason,
                        notes: reason == StockOutReason.Sale ? "Retail sale" : reason == StockOutReason.Damage ? "Damaged in handling" : "Internal office use");
                }

                // ~1/3 of products get a mid-period restock so the ledger shows repeat purchasing
                if (rng.Next(0, 3) == 0 && !lowStockTargets.Contains(p.SKU))
                {
                    var restockDate = today.AddDays(-rng.Next(10, 40));
                    int restockQty = p.ReorderThreshold + rng.Next(5, 15);
                    AddTransaction(p, TransactionType.StockIn, restockQty, restockDate, p.SupplierId, p.CostPrice, notes: "Replenishment order");
                }

                // For deliberate low-stock demo products, add one more draw-down close to today
                if (lowStockTargets.Contains(p.SKU))
                {
                    var date = today.AddDays(-rng.Next(1, 6));
                    int qty = Math.Max(0, p.CurrentStock - rng.Next(0, Math.Max(1, p.ReorderThreshold - 1)));
                    AddTransaction(p, TransactionType.StockOut, qty, date, reason: StockOutReason.Sale, notes: "Retail sale");
                }
            }

            context.StockTransactions.AddRange(transactions.OrderBy(t => t.TransactionDate));
            context.SaveChanges();
        }
    }
}
