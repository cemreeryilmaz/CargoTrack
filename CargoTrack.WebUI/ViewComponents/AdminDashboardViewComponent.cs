using CargoTrack.Business.Services.Dashboards;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    /// <summary>
    /// Admin dashboard verilerini dinamik olarak veritabanından besleyen ViewComponent.
    /// </summary>
    public class AdminDashboardViewComponent(
        IDashboardService _dashboardService,
        UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userCount = _userManager.Users.Count();
            var model = await _dashboardService.GetAdminDashboardAsync(userCount);
            return View(model);
        }
    }
}
