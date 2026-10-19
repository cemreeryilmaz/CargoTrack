using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.User.Controllers
{
    [Area(Area.User)]
    [Authorize(Roles = Roles.User)]
    public class DashboardController : Controller
    {
        // Veriler UserDashboard ViewComponent üzerinden dinamik olarak yüklenir.
        public IActionResult Index() => View();
    }
}
