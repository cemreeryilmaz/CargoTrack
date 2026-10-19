using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Manager.Controllers
{
    [Area(Area.Manager)]
    [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
    public class DashboardController : Controller
    {
        // Veriler ManagerDashboard ViewComponent üzerinden dinamik olarak yüklenir.
        public IActionResult Index() => View();
    }
}
