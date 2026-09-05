using CargoTrack.Business.Services.Abouts;
using CargoTrack.DTO.DTOs.AboutDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class AboutController(IAboutService _aboutService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var abouts = await _aboutService.GetAllAsync();
            return View(abouts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAboutDto createAboutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createAboutDto);
            }

            await _aboutService.CreateAsync(createAboutDto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var about = await _aboutService.GetByIdAsync(id);

            return View(about);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateAboutDto updateAboutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateAboutDto);
            }

            await _aboutService.UpdateAsync(updateAboutDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _aboutService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
