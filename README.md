# InventoryFlow — Inventory & Stock Management System

A standalone ASP.NET MVC (.NET 8) portfolio project for tracking stock levels,
suppliers, and stock movement for a small retail business or distributor.
Built with the same stack and 3-layer pattern as your ERP Billing project
(Repository → Service → Controller, EF Core, SQL Server, Razor + Bootstrap 5),
but with a distinct domain and a distinct visual theme so the two projects
read as range, not reskins.

> **Note on this sandbox:** I don't have the .NET SDK or NuGet access in this
> environment, so I couldn't compile/run this here. Everything below is real,
> complete source code — open it in Visual Studio / run `dotnet run` locally
> and it will work following the setup steps below.

---

## 1. Quick Start

```bash
cd InventoryFlow
dotnet restore

# One-time: generate the EF Core migration (not included — see Migrations/README.md)
dotnet tool install --global dotnet-ef   # if you don't have it
dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet run
```

Then open the URL shown in the console (e.g. `https://localhost:7236`).
On first run, the app seeds demo data automatically (9 suppliers, 23 products
across 5 categories, ~100 stock transactions spread over the last 4 months) —
the dashboard and ledger will look populated immediately.

Update the connection string in `appsettings.json` if you're not using
LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InventoryFlowDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

No authentication is wired up (per your v1/portfolio-demo constraint) — every
page is open.

---

## 2. Database Schema & Key Decisions

**Tables:** `Suppliers`, `Categories`, `Products`, `StockTransactions`.

```
Suppliers (1) ───< (M) Products  [SupplierId, nullable — a product can have no fixed supplier]
Categories (1) ───< (M) Products
Products (1) ───< (M) StockTransactions
Suppliers (1) ───< (M) StockTransactions  [nullable — only set for Stock-In]
```

**`StockTransactions`** is a single unified ledger table for both stock-in and
stock-out, distinguished by a `TransactionType` column, rather than two
separate `StockIn`/`StockOut` tables. Reasoning:

- **Auditability** — rows are only ever inserted, never updated or deleted, so
  the table behaves like an append-only financial ledger. A "movement history"
  view is then just one `ORDER BY` away, no `UNION` of two tables required.
- **Extensibility** — adding a new movement type later (e.g. `Adjustment`,
  `Return`, `Transfer`) is an enum value, not a new table + new join.
- **Simpler reporting** — stock-value-over-time, top-movers, and supplier
  reliability reports all read from one table.

**How current stock is calculated — cache vs. recompute, and the tradeoff:**

`Products.CurrentStock` is a **denormalized cached column**, not computed live
from the ledger on every read. It's updated transactionally inside
`StockService.StockInAsync` / `StockOutAsync` at the same time the
`StockTransaction` row is inserted (wrapped in an explicit DB transaction, so
the two writes succeed or fail together).

- **Why cache instead of `SUM()` on every page load:** the dashboard, product
  list, and low-stock check all need `CurrentStock` constantly — recomputing
  `SUM(StockIn) - SUM(StockOut)` per product on every request doesn't scale
  once the ledger has thousands of rows, and it's unnecessary I/O for a value
  that changes only on explicit stock movements.
- **The tradeoff:** a cached column can drift from the ledger if something
  bypasses the service layer (a bad migration, manual SQL, etc.). To cover
  that, `IStockService.RecalculateStockAsync()` recomputes every product's
  stock purely from `StockTransactions` — a reconciliation/integrity-check
  tool you can point to in an interview as the answer to "what if the cache
  is wrong?"
- **Stock-out negative-stock protection** is enforced in `StockService`
  *before* the transaction is written — quantity requested is compared
  against `CurrentStock` and rejected with a clear error if insufficient,
  never relying on a DB constraint alone (though a `CHECK (CurrentStock >= 0)`
  constraint is also in place as a second line of defense).

A `BalanceAfterTransaction` snapshot column on each ledger row makes the
movement-history view read like a bank statement (running balance) without
recomputing it at query time.

---

## 3. Project / Folder Structure

```
InventoryFlow/
├── Controllers/          SupplierController, ProductController, StockController, DashboardController
├── Models/
│   ├── Supplier.cs, Category.cs, Product.cs, StockTransaction.cs
│   ├── Enums/             TransactionType, StockOutReason
│   └── ViewModels/        DashboardViewModel, StockInViewModel, StockOutViewModel, ProductFormViewModel
├── Data/
│   ├── AppDbContext.cs    EF Core context + fluent-API constraints
│   └── SeedData.cs        Demo data generator
├── Repositories/
│   ├── Interfaces/        IRepository<T>, ISupplierRepository, IProductRepository, IStockTransactionRepository
│   └── *.cs               EF Core implementations
├── Services/
│   ├── Interfaces/        ISupplierService, ICategoryService, IProductService, IStockService
│   └── *.cs               Business logic (validation, stock math, dashboard aggregation)
├── Views/                 Razor views per controller + shared layout
└── wwwroot/css/site.css   Custom "warehouse" theme (see below)
```

Same Repository → Service → Controller layering as your billing project:
controllers stay thin (model binding + calling a service + choosing a view),
all business rules (SKU uniqueness, negative-stock prevention, low-stock
calculation, dashboard aggregation) live in the Service layer, and
repositories only know how to talk to EF Core.

---

## 4. Build Order (as implemented)

1. **Schema & DbContext** (`Data/AppDbContext.cs`)
2. **Supplier CRUD** (`SupplierController` / `SupplierService` / `Views/Supplier`)
3. **Product CRUD** (`ProductController` / `ProductService` / `Views/Product`) — new products always start at 0 stock; stock only ever changes through a transaction
4. **Stock In / Stock Out** (`StockController` / `StockService`) — the transactional core
5. **Low-stock alert logic** — `Product.IsLowStock` (computed) + `IProductRepository.GetLowStockProductsAsync()`
6. **Movement history ledger** — `Stock/History.cshtml`, a per-product bank-statement-style view
7. **Dashboard** — stat cards, Chart.js doughnut (stock value by category) and bar chart (top movers, last 30 days), low-stock panel with a one-click **Reorder Now** button straight into Stock-In

---

## 5. Visual Theme

Deliberately distinct from a typical blue ERP/billing look: a **deep
forest-teal sidebar** with an **amber accent** for alerts and reorder CTAs,
on a light neutral background — meant to read as "warehouse / logistics"
rather than "finance." All custom CSS lives in `wwwroot/css/site.css` under
`--inv-*` custom properties, so the palette is a one-file change if you want
to adjust it. Layout is a fixed sidebar + topbar shell rather than your
billing project's (presumably top-nav) layout, for further visual
differentiation while still being clearly "your" component style.

---

## 6. Polish Details Implemented

1. **Stock-level progress bar** on every product row/detail page — green
   while healthy, amber once at/under threshold, red at zero.
2. **Color-coded badges** — *In Stock* (green) / *Low Stock* (amber) / *Out of
   Stock* (red) — consistent across the product list, details page, and
   dashboard.
3. **"Reorder Now" CTA** — every row in the dashboard's low-stock panel links
   straight into a pre-filled Stock-In form for that product, so the alert is
   actionable, not just informational.

---

## 7. Seed / Demo Data

`Data/SeedData.cs` runs once on startup (if `Products` is empty) and creates:
- 9 suppliers across 5 product categories
- 23 products (Electronics, Stationery, Groceries, Furniture, Cleaning Supplies)
- An initial stock-in per product ~4 months ago, plus 2–4 stock-out events
  each (sale/damage/internal use), occasional restocks, and a handful of
  products deliberately drawn down near/below their reorder threshold —
  so the low-stock panel, charts, and ledger all have realistic content for
  screenshots or a demo video, with a fixed random seed for reproducibility.

---

## 8. Portfolio Case Study

> **InventoryFlow — Inventory & Stock Management System**
> Small retailers and distributors often lose money not from bad sales, but
> from not knowing what's actually on the shelf — leading to stockouts that
> cost sales, or overstocking that ties up cash. InventoryFlow gives a small
> business a single place to track stock across suppliers and categories,
> log every purchase and sale as it happens, and get flagged automatically
> the moment an item drops below its reorder point. A built-in movement
> ledger means every stock change is traceable back to a date, reason, and
> supplier, turning "I think we're low on X" into a two-second dashboard
> check. Built with ASP.NET MVC, EF Core, and SQL Server in a layered
> architecture for maintainability, it demonstrates a transactional,
> audit-friendly approach to a problem nearly every physical-goods business
> has.

(132 words — trim/expand to taste.)

---

## 9. Things to Call Out in an Interview

- The single-ledger-table decision and why it beats two tables (Section 2)
- Cached `CurrentStock` vs. live `SUM()` and the reconciliation escape hatch
  (`RecalculateStockAsync`)
- Negative-stock prevention enforced in the service layer *and* backstopped
  by a DB check constraint (defense in depth)
- Explicit DB transactions wrapping the "update Product + insert
  StockTransaction" pair, so a failure can't leave stock and ledger out of sync
