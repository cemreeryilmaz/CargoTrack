using CargoTrack.Business.Services.Dashboards;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    /// <summary>
    /// User dashboard verilerini (yalnızca kendi kargoları) besleyen ViewComponent.
    /// </summary>
    public class UserDashboardViewComponent(
        IDashboardService _dashboardService,
        UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User)!);
            var model = await _dashboardService.GetUserDashboardAsync(userId);
            return View(model);
        }
    }
}
