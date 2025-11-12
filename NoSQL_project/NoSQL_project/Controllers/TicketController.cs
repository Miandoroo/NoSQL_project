using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
using NoSQL_project.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
// Acepta /Ticket y /Tickets como base
[Route("Ticket")]
[Route("Tickets")]
public class TicketController : Controller
{
    private readonly ITicketService _svc;
    public TicketController(ITicketService svc) => _svc = svc;

    // --- Service Desk (todos) ---

    // GET /Ticket    y /Tickets
    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("")]
    public async Task<IActionResult> Index(int days = 7)
    {
        var list = await _svc.GetLastDaysAggAsync(days);
        ViewBag.Days = days;
        return View(list);
    }

    // GET /Ticket/Details/{id}
    [HttpGet("Details/{id}")]
    [Authorize(Policy = "ServiceDeskOnly")]
    public async Task<IActionResult> Details(string id) =>
        View(await _svc.GetByIdAsync(id));

    // GET /Ticket/Create
    [HttpGet("Create")]
    [Authorize(Policy = "ServiceDeskOnly")]
    public IActionResult Create() => View();

    // POST /Ticket/Create
    [HttpPost("Create")]
    [Authorize(Policy = "ServiceDeskOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Tickets t)
    {
        ModelState.Remove(nameof(Tickets.Id));
        ModelState.Remove(nameof(Tickets.UserId)); // quítalo si aún no seleccionas usuario
        
        if (!ModelState.IsValid) return View(t);

        // Si no seleccionas usuario en el form, usa el propio SD o algún valor por defecto
        t.UserId = t.UserId ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (t.Date == default) t.Date = DateTime.UtcNow.Date;
        if (t.Deadline == default) t.Deadline = t.Date.AddDays(7);
        TempData["Success"] = "Ticket created.";
        TempData["Error"] = "An error occurred while saving the ticket.";
        await _svc.CreateAsync(t);
        return RedirectToAction(nameof(Index));
    }

    // GET /Ticket/Edit/{id}
    [HttpGet("Edit/{id}")]
    [Authorize(Policy = "ServiceDeskOnly")]
    public async Task<IActionResult> Edit(string id) => View(await _svc.GetByIdAsync(id));

    // POST /Ticket/Edit/{id}
    [HttpPost("Edit/{id}")]
    [Authorize(Policy = "ServiceDeskOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Tickets t)
    {
        if (!ModelState.IsValid) return View(t);
        await _svc.UpdateAsync(id, t);
        return RedirectToAction(nameof(Index));
    }

    // GET /Ticket/Delete/{id}
    [HttpGet("Delete/{id}")]
    [Authorize(Policy = "ServiceDeskOnly")]
    public async Task<IActionResult> Delete(string id) => View(await _svc.GetByIdAsync(id));

    // POST /Ticket/Delete/{id}
    [HttpPost("Delete/{id}")]
    [ActionName("Delete")]
    [Authorize(Policy = "ServiceDeskOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _svc.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // --- Empleado regular (sus tickets) ---

    // GET /Ticket/My   y /Tickets/My
    [HttpGet("My")]
    public async Task<IActionResult> My(int days = 7)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var list = await _svc.GetByUserLastDaysAggAsync(uid, days);
        ViewBag.Days = days;
        return View("Index", list);
    }

    // GET /Ticket/CreateMine
    [HttpGet("CreateMine")]
    public IActionResult CreateMine() => View("Create");

    // POST /Ticket/CreateMine
    [HttpPost("CreateMine")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMine([FromForm] Tickets t)
    {
        // Estos los pones tú, así que quítalos de la validación
        ModelState.Remove(nameof(Tickets.Id));
        ModelState.Remove(nameof(Tickets.UserId));

        if (!ModelState.IsValid) return View("Create", t);

        t.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (t.Date == default) t.Date = DateTime.UtcNow.Date;
        if (t.Deadline == default) t.Deadline = t.Date.AddDays(7);

        await _svc.CreateAsync(t);
        return RedirectToAction(nameof(My));
    }

}
