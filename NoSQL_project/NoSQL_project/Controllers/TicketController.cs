using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;

namespace NoSQL_project.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _repo;
        public TicketController(ITicketRepository repo) => _repo = repo;

       public IActionResult Index()
       {
         List<Tickets> tickets = _repo.GetAll();
         return View(tickets);    
       }

        [HttpGet]
        public IActionResult Create(string userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tickets ticket, string userId)
        {
            ticket.Id = ObjectId.GenerateNewId().ToString();
            ticket.UserId = userId;

            _repo.Add(ticket);
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            Tickets tickets = _repo.GetById(id);
            return View(tickets);
        }

        [HttpPost]
        public IActionResult Update(string id, Tickets tickets)
        {


            if (!ModelState.IsValid)
            {
                return View(tickets);
            }
            _repo.Update(id, tickets);
            TempData["Success"] = "Successfully edited ticket";
            return RedirectToAction(nameof(Index));
        }
    }
}
