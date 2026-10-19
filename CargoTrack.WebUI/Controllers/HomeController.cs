using CargoTrack.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CargoTrack.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Default");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Yetkisiz erişim ekranı
        public IActionResult AccessDenied()
        {
            return View();
        }

        // İş kuralı (ValidationException) kaynaklı yakalanan hatalar
        public IActionResult HandledError(string? message)
        {
            ViewBag.Message = message ?? "Beklenmeyen bir hata oluştu.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
