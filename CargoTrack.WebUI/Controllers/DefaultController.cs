using CargoTrack.Business.Services.Abouts;
using CargoTrack.Business.Services.Branchs;
using CargoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    /// <summary>
    /// Giriş gerektirmeyen public ekranlar: ana sayfa, kargo takip, kurumsal sayfalar.
    /// </summary>
    public class DefaultController(
        ICargoService _cargoService,
        IBranchService _branchService,
        IAboutService _aboutService) : Controller
    {
        // Ana sayfa + son kargolar
        public async Task<IActionResult> Index()
        {
            if (TempData["error"] != null)
                ViewBag.error = TempData["error"];

            ViewBag.Recent = await _cargoService.GetRecentAsync(6);
            return View();
        }

        // Kargo takip sonucu (zaman çizelgesi)
        public async Task<IActionResult> CargoDetails(string trackCode)
        {
            if (string.IsNullOrWhiteSpace(trackCode))
            {
                TempData["error"] = "Lütfen bir takip numarası giriniz.";
                return RedirectToAction(nameof(Index));
            }

            var tracking = await _cargoService.GetTrackingAsync(trackCode);

            if (tracking is null)
            {
                TempData["error"] = "Takip numarasına ait kargo bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(tracking);
        }

        // Şubelerimiz
        public async Task<IActionResult> Branches()
        {
            var branches = await _branchService.GetAllAsync();
            return View(branches);
        }

        // Hizmetlerimiz (bilgilendirme)
        public IActionResult Services() => View();

        // Hakkımızda
        public async Task<IActionResult> About()
        {
            var abouts = await _aboutService.GetAllAsync();
            return View(abouts);
        }

        // İletişim
        public IActionResult Contact() => View();

        // Gizlilik / KVKK
        public IActionResult Privacy() => View();
    }
}
