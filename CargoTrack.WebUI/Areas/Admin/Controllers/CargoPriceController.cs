using CargoTrack.Business.Services.CargoPrices;
using CargoTrack.DTO.DTOs.CargoPriceDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class CargoPriceController(ICargoPriceService _cargoPriceService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var list = await _cargoPriceService.GetAllAsync();
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateCargoPriceDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _cargoPriceService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var dto = await _cargoPriceService.GetByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCargoPriceDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _cargoPriceService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _cargoPriceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
