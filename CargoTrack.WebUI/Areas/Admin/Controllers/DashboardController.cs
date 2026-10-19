using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class DashboardController : Controller
    {
        // Veriler AdminDashboard ViewComponent üzerinden dinamik olarak yüklenir.
        public IActionResult Index() => View();
    }
}
