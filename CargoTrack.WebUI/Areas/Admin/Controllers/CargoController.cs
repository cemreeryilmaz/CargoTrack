using CargoTrack.Business.Services.Branchs;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class CargoController(
        ICargoService _cargoService,
        IBranchService _branchService,
        UserManager<AppUser> _userManager) : Controller
    {
        // Admin tüm kargoları görür (pagination + search + filter)
        public async Task<IActionResult> Index(CargoFilterDto filter)
        {
            var result = await _cargoService.GetPagedAsync(filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            return View(cargo);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCargoDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(dto);
            }

            await _cargoService.CreateAsync(dto, GetUserId(), User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadDropdownsAsync();
            var dto = await _cargoService.GetByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCargoDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(dto);
            }

            await _cargoService.UpdateAsync(dto, GetUserId(), User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _cargoService.DeleteAsync(id, GetUserId(), User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        private Guid? GetUserId()
        {
            var id = _userManager.GetUserId(User);
            return id is null ? null : Guid.Parse(id);
        }

        private async Task LoadDropdownsAsync()
        {
            var branches = await _branchService.GetAllAsync();
            ViewBag.Branches = branches
                .OrderBy(b => b.Name)
                .Select(b => new SelectListItem { Text = $"{b.Name} ({b.City?.Name})", Value = b.Id.ToString() })
                .ToList();

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users
                .Select(u => new SelectListItem { Text = $"{u.FirstName} {u.LastName} ({u.UserName})", Value = u.Id.ToString() })
                .ToList();
        }
    }
}
