using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.DTO.DTOs.DeliveryDtos;
using CargoTrack.DTO.DTOs.DeliveryExceptionDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Manager.Controllers
{
    [Area(Area.Manager)]
    [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
    public class CargoController(
        ICargoService _cargoService,
        IEmployeeService _employeeService,
        ITransferCenterService _transferCenterService,
        UserManager<AppUser> _userManager) : Controller
    {
        // Manager yalnızca kendi şubesine gelen/çıkan kargoları görür.
        public async Task<IActionResult> Index(CargoFilterDto filter)
        {
            var branchId = await GetBranchIdAsync();
            if (branchId is null) return BranchNotAssigned();

            var result = await _cargoService.GetBranchPagedAsync(branchId.Value, filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            return View(cargo);
        }

        // Yeni hareket girişi (durum güncelleme)
        public async Task<IActionResult> NewMovement(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            await LoadNextStatusesAsync(cargo.CargoStatus);
            await LoadTransferCentersAsync();

            ViewBag.Cargo = cargo;
            return View(new ChangeCargoStatusDto { CargoId = id });
        }

        [HttpPost]
        public async Task<IActionResult> NewMovement(ChangeCargoStatusDto dto)
        {
            var branchId = await GetBranchIdAsync();
            var employeeId = await GetEmployeeIdAsync();

            dto.BranchId ??= branchId;

            await _cargoService.ChangeStatusAsync(dto, employeeId, GetUserId(), User.Identity?.Name);
            return RedirectToAction(nameof(Detail), new { id = dto.CargoId });
        }

        // Dağıtım listesi (dağıtıma çıkmış kargolar)
        public async Task<IActionResult> Distribution()
        {
            var branchId = await GetBranchIdAsync();
            if (branchId is null) return BranchNotAssigned();

            var filter = new CargoFilterDto { Status = CargoStatus.OutForDelivery, PageSize = 100 };
            var result = await _cargoService.GetBranchPagedAsync(branchId.Value, filter);
            return View(result);
        }

        // Teslimat kodu doğrulama
        public async Task<IActionResult> ConfirmDelivery(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            ViewBag.Cargo = cargo;
            return View(new ConfirmDeliveryDto { CargoId = id });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelivery(ConfirmDeliveryDto dto)
        {
            var employeeId = await GetEmployeeIdAsync();
            await _cargoService.ConfirmDeliveryAsync(dto, employeeId, GetUserId(), User.Identity?.Name);
            return RedirectToAction(nameof(Detail), new { id = dto.CargoId });
        }

        // Sorunlu kargo / başarısız teslimat kaydı
        public async Task<IActionResult> ReportException(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            ViewBag.Cargo = cargo;
            LoadExceptionReasons();
            return View(new CreateDeliveryExceptionDto { CargoId = id });
        }

        [HttpPost]
        public async Task<IActionResult> ReportException(CreateDeliveryExceptionDto dto)
        {
            var employeeId = await GetEmployeeIdAsync();
            await _cargoService.ReportDeliveryFailureAsync(dto, employeeId, GetUserId(), User.Identity?.Name);
            return RedirectToAction(nameof(Detail), new { id = dto.CargoId });
        }

        // ---------------- Helpers ----------------
        private Guid? GetUserId()
        {
            var id = _userManager.GetUserId(User);
            return id is null ? null : Guid.Parse(id);
        }

        private async Task<Guid?> GetBranchIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.BranchId;
        }

        private async Task<Guid?> GetEmployeeIdAsync()
        {
            var userId = GetUserId();
            if (userId is null) return null;
            var employee = await _employeeService.GetByAppUserIdAsync(userId.Value);
            return employee?.Id;
        }

        private IActionResult BranchNotAssigned()
        {
            ViewBag.NoBranch = true;
            return View("Index", new CargoTrack.Entity.Entities.Common.PagedResult<ResultCargoDto>());
        }

        private async Task LoadTransferCentersAsync()
        {
            var centers = await _transferCenterService.GetAllAsync();
            ViewBag.TransferCenters = centers
                .Select(t => new SelectListItem { Text = $"{t.Name} ({t.CityName})", Value = t.Id.ToString() })
                .ToList();
        }

        private Task LoadNextStatusesAsync(CargoStatus current)
        {
            var next = CargoTrack.Business.Services.Cargos.CargoStatusRules.GetNextStatuses(current)
                .Where(s => s != CargoStatus.Delivered && s != CargoStatus.DeliveryFailed)
                .Select(s => new SelectListItem { Text = s.GetDisplayName(), Value = ((int)s).ToString() })
                .ToList();

            ViewBag.NextStatuses = next;
            return Task.CompletedTask;
        }

        private void LoadExceptionReasons()
        {
            ViewBag.Reasons = Enum.GetValues<DeliveryExceptionReason>()
                .Select(r => new SelectListItem { Text = r.GetDisplayName(), Value = ((int)r).ToString() })
                .ToList();
        }
    }
}
