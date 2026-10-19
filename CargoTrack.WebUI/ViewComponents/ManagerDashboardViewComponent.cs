using CargoTrack.Business.Services.Dashboards;
using CargoTrack.DTO.DTOs.DashboardDtos;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    /// <summary>
    /// Manager dashboard verilerini (yalnızca bağlı şube) besleyen ViewComponent.
    /// </summary>
    public class ManagerDashboardViewComponent(
        IDashboardService _dashboardService,
        UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user?.BranchId is null)
                return View(new ManagerDashboardDto { BranchName = "-" });

            var model = await _dashboardService.GetManagerDashboardAsync(user.BranchId.Value);
            return View(model);
        }
    }
}
