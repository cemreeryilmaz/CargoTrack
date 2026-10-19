using CargoTrack.Business.Services.Cargos;
using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.User.Controllers
{
    [Area(Area.User)]
    [Authorize(Roles = Roles.User)]
    public class CargoController(
        ICargoService _cargoService,
        UserManager<AppUser> _userManager) : Controller
    {
        // User yalnızca kendi gönderdiği/aldığı kargoları görür.
        public async Task<IActionResult> Index(CargoFilterDto filter)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var result = await _cargoService.GetUserPagedAsync(userId, filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            return View(cargo);
        }

        // ---------------- Teslimat Tercihleri (kargo dağıtıma çıktığında) ----------------

        // "Adresteyim"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> AtAddress(Guid id)
            => SetPreferenceAsync(id, DeliveryPreference.AtAddress, null, null);

        // "Komşuma teslim et" — komşu adresi zorunlu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> DeliverToNeighbor(Guid id, string neighborAddress)
            => SetPreferenceAsync(id, DeliveryPreference.DeliverToNeighbor, neighborAddress, null);

        // "Başka bir güne randevu oluştur" — tarih zorunlu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CreateAppointment(Guid id, DateTime appointmentDate)
            => SetPreferenceAsync(id, DeliveryPreference.Appointment, null, appointmentDate);

        private async Task<IActionResult> SetPreferenceAsync(
            Guid id, DeliveryPreference preference, string? neighborAddress, DateTime? appointmentDate)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var userName = User.Identity?.Name;
            try
            {
                await _cargoService.SetDeliveryPreferenceAsync(
                    id, userId, preference, neighborAddress, appointmentDate, userName);
                TempData["Success"] = "Teslimat tercihiniz kaydedildi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
