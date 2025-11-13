using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
using NoSQL_project.Services.Interfaces;
using System.Security.Claims;

namespace NoSQL_project.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        public TicketController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        private string? GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private bool IsServiceDesk() => User.IsInRole("ServiceDeskEmployee");

        public IActionResult Index(string searchQuery)
        {
            var tickets = _ticketService.GetTicketsForUser(GetCurrentUserId(), IsServiceDesk(), searchQuery);
            ViewBag.SearchQuery = searchQuery;
            return View(tickets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = IsServiceDesk();
            ViewBag.CurrentUserId = GetCurrentUserId();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            ModelState.Remove(nameof(Ticket.Id));
            if (ModelState.IsValid)
            {
                try
                {
                    _ticketService.Create(ticket, GetCurrentUserId(), IsServiceDesk());
                    TempData["Success"] = "Ticket successfully created!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("UserId", ex.Message);
                }
            }

            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = IsServiceDesk();
            ViewBag.CurrentUserId = GetCurrentUserId();
            return View(ticket);
        }

        public IActionResult Details(string id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
                return NotFound();

            if (!_ticketService.CanAccessTicket(ticket, GetCurrentUserId(), IsServiceDesk()))
                return Forbid();

            ViewBag.User = _ticketService.GetTicketUser(id);
            return View(ticket);
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
                return NotFound();

            if (!_ticketService.CanAccessTicket(ticket, GetCurrentUserId(), IsServiceDesk()))
                return Forbid();

            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = IsServiceDesk();
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string id, Ticket ticket, string UserId)
        {
            var existingTicket = _ticketService.GetById(id);
            if (existingTicket == null)
                return NotFound();

            if (!_ticketService.CanAccessTicket(existingTicket, GetCurrentUserId(), IsServiceDesk()))
                return Forbid();

            if (ModelState.IsValid)
            {
                _ticketService.Update(id, ticket, UserId, IsServiceDesk());
                TempData["Success"] = "Successfully edited ticket";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = IsServiceDesk();
            return View(ticket);
        }

        [HttpGet]
        [Authorize(Roles = "ServiceDeskEmployee")]
        public IActionResult Delete(string id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
                return NotFound();
            
            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ServiceDeskEmployee")]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _ticketService.Delete(id);
                TempData["Success"] = "Ticket successfully deleted!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}
