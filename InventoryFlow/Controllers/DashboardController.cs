using InventoryFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryFlow.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IStockService _stockService;

        public DashboardController(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _stockService.GetDashboardDataAsync();
            return View(data);
        }
    }

    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Dashboard");

        public IActionResult Error() => View();
    }
}
