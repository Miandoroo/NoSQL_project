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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Ticket> tickets;
            if (User.IsInRole("ServiceDeskEmployee"))
            {
                tickets = _ticketService.GetAll();
            }
            else
            {
                tickets = _ticketService.GetByUserId(userId);
            }

            var totalTickets = tickets.Count;
            var openTickets = tickets.Count(t => t.Status == 0 || t.Status == 1);
            var resolvedTickets = tickets.Count(t => t.Status == 2);
            var closedTickets = tickets.Count(t => t.Status == 3);

            var dashboardModel = new DashboardViewModel
            {
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                ResolvedTickets = resolvedTickets,
                ClosedTickets = closedTickets,
                PercentOpen = totalTickets > 0 ? (openTickets * 100.0 / totalTickets) : 0,
                PercentResolved = totalTickets > 0 ? (resolvedTickets * 100.0 / totalTickets) : 0,
                PercentClosed = totalTickets > 0 ? (closedTickets * 100.0 / totalTickets) : 0,
                RecentTickets = tickets.OrderByDescending(t => t.Date).Take(5).ToList(),
                IsServiceDesk = User.IsInRole("ServiceDeskEmployee")
            };

            return View(dashboardModel);
        }
    }
}
