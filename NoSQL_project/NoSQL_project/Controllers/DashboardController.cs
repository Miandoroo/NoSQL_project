using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Services.Interfaces;
using NoSQL_project.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace NoSQL_project.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITicketService _svc;
        public DashboardController(ITicketService svc) => _svc = svc;

        // /Dashboard/All?days=7   (Service Desk)
        [HttpGet("All")]
        [Authorize(Policy = "ServiceDeskOnly")]
        public async Task<IActionResult> All(int days = 7)
        {
            var vm = await _svc.GetDashboardAsync(days, userId: null, isAll: true);
            return View("Stats", vm);
        }

        // /Dashboard/My?days=7    (Empleado)
        [HttpGet("My")]
        public async Task<IActionResult> My(int days = 7)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = await _svc.GetDashboardAsync(days, userId: uid, isAll: false);
            return View("Stats", vm);
        }
    }
}
