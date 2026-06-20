using InventoryFlow.Models.ViewModels;
using InventoryFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryFlow.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;

        public StockController(IStockService stockService, IProductService productService, ISupplierService supplierService)
        {
            _stockService = stockService;
            _productService = productService;
            _supplierService = supplierService;
        }

        // GET: /Stock/In
        public async Task<IActionResult> In(int? productId)
        {
            var vm = new StockInViewModel
            {
                Products = (await _productService.GetAllAsync()).Where(p => p.IsActive).ToList(),
                Suppliers = (await _supplierService.GetAllAsync()).Where(s => s.IsActive).ToList(),
                ProductId = productId ?? 0
            };
            return View(vm);
        }

        // POST: /Stock/In
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> In(StockInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(await RebuildStockIn(model));

            var (success, error) = await _stockService.StockInAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to record stock-in entry.");
                return View(await RebuildStockIn(model));
            }

            TempData["Success"] = $"Stock-in of {model.Quantity} unit(s) recorded successfully.";
            return RedirectToAction("History", new { productId = model.ProductId });
        }

        // GET: /Stock/Out
        public async Task<IActionResult> Out(int? productId)
        {
            var vm = new StockOutViewModel
            {
                Products = (await _productService.GetAllAsync()).Where(p => p.IsActive).ToList(),
                ProductId = productId ?? 0
            };
            if (productId.HasValue)
            {
                var product = await _productService.GetByIdAsync(productId.Value);
                vm.AvailableStock = product?.CurrentStock ?? 0;
            }
            return View(vm);
        }

        // POST: /Stock/Out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Out(StockOutViewModel model)
        {
            if (!ModelState.IsValid)
                return View(await RebuildStockOut(model));

            var (success, error) = await _stockService.StockOutAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to record stock-out entry.");
                return View(await RebuildStockOut(model));
            }

            TempData["Success"] = $"Stock-out of {model.Quantity} unit(s) recorded successfully.";
            return RedirectToAction("History", new { productId = model.ProductId });
        }

        // GET: /Stock/History/5  — per-product movement ledger
        public async Task<IActionResult> History(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null) return NotFound();

            ViewBag.Product = product;
            var history = await _stockService.GetHistoryForProductAsync(productId);
            return View(history);
        }

        private async Task<StockInViewModel> RebuildStockIn(StockInViewModel model)
        {
            model.Products = (await _productService.GetAllAsync()).Where(p => p.IsActive).ToList();
            model.Suppliers = (await _supplierService.GetAllAsync()).Where(s => s.IsActive).ToList();
            return model;
        }

        private async Task<StockOutViewModel> RebuildStockOut(StockOutViewModel model)
        {
            model.Products = (await _productService.GetAllAsync()).Where(p => p.IsActive).ToList();
            var product = await _productService.GetByIdAsync(model.ProductId);
            model.AvailableStock = product?.CurrentStock ?? 0;
            return model;
        }
    }
}
