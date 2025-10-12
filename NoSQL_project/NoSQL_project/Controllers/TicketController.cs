using Microsoft.AspNetCore.Mvc;
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
    }
}
