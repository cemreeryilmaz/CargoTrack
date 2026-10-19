using CargoTrack.Business.Services.Branchs;
using CargoTrack.Business.Services.Employees;
using CargoTrack.DTO.DTOs.EmployeeDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class EmployeeController(
        IEmployeeService _employeeService,
        IBranchService _branchService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var list = await _employeeService.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            await LoadBranchesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadBranchesAsync();
                return View(dto);
            }

            await _employeeService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadBranchesAsync();
            var dto = await _employeeService.GetByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadBranchesAsync();
                return View(dto);
            }

            await _employeeService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadBranchesAsync()
        {
            var branches = await _branchService.GetAllAsync();
            ViewBag.Branches = branches
                .OrderBy(b => b.Name)
                .Select(b => new SelectListItem { Text = $"{b.Name} ({b.City?.Name})", Value = b.Id.ToString() })
                .ToList();
        }
    }
}
