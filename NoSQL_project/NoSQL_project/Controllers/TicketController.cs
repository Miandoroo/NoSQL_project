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

            return View(tickets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = User.IsInRole("ServiceDeskEmployee");
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            ModelState.Remove(nameof(Ticket.Id));
            if (ModelState.IsValid)
            {
                if (!User.IsInRole("ServiceDeskEmployee"))
                {
                    ticket.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                else if (string.IsNullOrEmpty(ticket.UserId))
                {
                    ModelState.AddModelError("UserId", "Please select a user");
                    ViewBag.Users = _userService.GetAll();
                    ViewBag.IsServiceDesk = User.IsInRole("ServiceDeskEmployee");
                    ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    return View(ticket);
                }

                _ticketService.Create(ticket);
                TempData["Success"] = "Ticket successfully created!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = User.IsInRole("ServiceDeskEmployee");
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(ticket);
        }

        public IActionResult Details(string id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("ServiceDeskEmployee") && ticket.UserId != userId)
                return Forbid();

            var user = _userService.GetById(ticket.UserId);
            ViewBag.User = user;
            return View(ticket);
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("ServiceDeskEmployee") && ticket.UserId != userId)
                return Forbid();

            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = User.IsInRole("ServiceDeskEmployee");
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string id, Ticket ticket, string UserId)
        {
            var existingTicket = _ticketService.GetById(id);
            if (existingTicket == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("ServiceDeskEmployee") && existingTicket.UserId != userId)
                return Forbid();

            if (ModelState.IsValid)
            {
                ticket.Id = id;
                if (User.IsInRole("ServiceDeskEmployee") && !string.IsNullOrEmpty(UserId))
                {
                    ticket.UserId = UserId;
                }
                else
                {
                    ticket.UserId = existingTicket.UserId;
                }
                _ticketService.Update(id, ticket);
                TempData["Success"] = "Successfully edited ticket";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _userService.GetAll();
            ViewBag.IsServiceDesk = User.IsInRole("ServiceDeskEmployee");
            return View(ticket);
        }

        [HttpGet]
        [Authorize(Roles = "ServiceDeskEmployee")]
        public IActionResult Delete(string id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }
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
