using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models.ViewModels;
using NoSQL_project.Services.Interfaces;
using System.Security.Claims;

namespace NoSQL_project.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITicketService _ticketService;

        public DashboardController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult Index()
        {
            var model = _ticketService.DashboardEmployee(
                User.IsInRole("ServiceDeskEmployee"),
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(model);
        }
    }
}
