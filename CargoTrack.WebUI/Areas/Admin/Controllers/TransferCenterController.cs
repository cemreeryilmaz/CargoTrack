using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.TransferCenterDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class TransferCenterController(
        ITransferCenterService _transferCenterService,
        ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var list = await _transferCenterService.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferCenterDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(dto);
            }

            await _transferCenterService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadCitiesAsync();
            var dto = await _transferCenterService.GetByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTransferCenterDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(dto);
            }

            await _transferCenterService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _transferCenterService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();
            ViewBag.Cities = cities
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
                .ToList();
        }
    }
}
