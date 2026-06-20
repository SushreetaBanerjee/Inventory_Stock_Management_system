using InventoryFlow.Models;
using InventoryFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryFlow.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: /Supplier
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return View(suppliers);
        }

        // GET: /Supplier/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // GET: /Supplier/Create
        public IActionResult Create() => View(new Supplier());

        // POST: /Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid) return View(supplier);

            var (success, error) = await _supplierService.CreateAsync(supplier);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to create supplier.");
                return View(supplier);
            }

            TempData["Success"] = $"Supplier '{supplier.Name}' was added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Supplier/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // POST: /Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId) return NotFound();
            if (!ModelState.IsValid) return View(supplier);

            var (success, error) = await _supplierService.UpdateAsync(supplier);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to update supplier.");
                return View(supplier);
            }

            TempData["Success"] = $"Supplier '{supplier.Name}' was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Supplier/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // POST: /Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, error) = await _supplierService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = success ? "Supplier deleted." : error;
            return RedirectToAction(nameof(Index));
        }
    }
}
