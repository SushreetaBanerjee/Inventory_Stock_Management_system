using InventoryFlow.Models;
using InventoryFlow.Models.ViewModels;
using InventoryFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryFlow.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;

        public ProductController(IProductService productService, ICategoryService categoryService, ISupplierService supplierService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
        }

        // GET: /Product
        public async Task<IActionResult> Index(string? search, int? categoryId, bool lowStockOnly = false)
        {
            var products = (await _productService.GetAllAsync()).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.SKU.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId);

            if (lowStockOnly)
                products = products.Where(p => p.IsLowStock);

            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.LowStockOnly = lowStockOnly;

            return View(products.ToList());
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: /Product/Create
        public async Task<IActionResult> Create()
        {
            var vm = new ProductFormViewModel
            {
                Categories = (await _categoryService.GetAllAsync()).ToList(),
                Suppliers = (await _supplierService.GetAllAsync()).ToList()
            };
            return View(vm);
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(await RebuildFormViewModel(product));

            var (success, error) = await _productService.CreateAsync(product);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to create product.");
                return View(await RebuildFormViewModel(product));
            }

            TempData["Success"] = $"Product '{product.Name}' was added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(await RebuildFormViewModel(product));
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();
            if (!ModelState.IsValid)
                return View(await RebuildFormViewModel(product));

            var (success, error) = await _productService.UpdateAsync(product);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to update product.");
                return View(await RebuildFormViewModel(product));
            }

            TempData["Success"] = $"Product '{product.Name}' was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, error) = await _productService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = success ? "Product deleted." : error;
            return RedirectToAction(nameof(Index));
        }

        private async Task<ProductFormViewModel> RebuildFormViewModel(Product product) => new()
        {
            Product = product,
            Categories = (await _categoryService.GetAllAsync()).ToList(),
            Suppliers = (await _supplierService.GetAllAsync()).ToList()
        };
    }
}
