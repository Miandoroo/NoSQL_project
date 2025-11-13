using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
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
           bool isServiceDesk = User.IsInRole("ServiceDeskEmployee");
           string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            DashboardViewModel model = _ticketService.DashboardEmployee(isServiceDesk, userId);
           return View(model);
        }
    }
}
