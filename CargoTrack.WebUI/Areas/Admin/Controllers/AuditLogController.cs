using CargoTrack.Business.Services.AuditLogs;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class AuditLogController(IAuditLogService _auditLogService) : Controller
    {
        public async Task<IActionResult> Index(int page = 1)
        {
            var result = await _auditLogService.GetPagedAsync(page, 20);
            return View(result);
        }
    }
}
