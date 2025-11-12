using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;

namespace NoSQL_project.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketReportService _svc;
        public TicketController(ITicketReportService svc) => _svc = svc;

        [HttpGet("/Reports/ByStatus")]
        public async Task<IActionResult> ByStatus()
        {
            var data = await _svc.ByStatusAsync();
            return View(data); // @model List<BsonDocument>
        }

        [HttpGet("/Reports/ByLocation")]
        public async Task<IActionResult> ByLocation()
        {
            var data = await _svc.ByUserLocationAsync();
            return View(data);
        }
    }
}
