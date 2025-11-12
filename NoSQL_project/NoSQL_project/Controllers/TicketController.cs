using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
using NoSQL_project.Services.Interfaces;
using System.Security.Claims;

[Authorize]
// Deja UNA base. (Si quieres soportar ambos, puedes duplicar: [Route("Ticket")], [Route("Tickets")])
[Route("Ticket")]
public class TicketController : Controller
{
    private readonly ITicketService _svc;
    public TicketController(ITicketService svc) => _svc = svc;

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("")]
    public async Task<IActionResult> Index() => View(await _svc.GetAllAsync());

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(string id) => View(await _svc.GetByIdAsync(id));

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("Create")]
    public IActionResult Create() => View();

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Tickets t)
    {
        ModelState.Remove(nameof(Tickets.Id));
        ModelState.Remove(nameof(Tickets.UserId));

        if (!ModelState.IsValid) return View(t);

        t.UserId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (t.Date == default) t.Date = DateTime.UtcNow.Date;
        if (t.Deadline == default) t.Deadline = t.Date.AddDays(7);

        await _svc.CreateAsync(t);
        TempData["Success"] = "Ticket created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id) => View(await _svc.GetByIdAsync(id));

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Tickets t)
    {
        if (!ModelState.IsValid) return View(t);
        await _svc.UpdateAsync(id, t);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(string id) => View(await _svc.GetByIdAsync(id));

    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpPost("Delete/{id}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _svc.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // --------- Mis tickets ----------
    [HttpGet("My")]
    public async Task<IActionResult> My()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var list = await _svc.GetByUserIdAsync(uid);
        return View("Index", list);
    }

    // ✅ GET routable para /Ticket/CreateMine
    [HttpGet("CreateMine")]
    public IActionResult CreateMine()
    {
        var m = new Tickets
        {
            Date = DateTime.UtcNow.Date,
            Deadline = DateTime.UtcNow.Date.AddDays(7),
            Status = 0,
            Priority = "Normal",
            IncidentType = "Request"
        };
        return View("Create", m);
    }

    // POST /Ticket/CreateMine
    [HttpPost("CreateMine")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMine(Tickets t)
    {
        ModelState.Remove(nameof(Tickets.Id));
        ModelState.Remove(nameof(Tickets.UserId));
        if (t.Date == default) t.Date = DateTime.UtcNow.Date;
        if (t.Deadline == default) t.Deadline = t.Date.AddDays(7);
        if (!ModelState.IsValid) return View("Create", t);

        t.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _svc.CreateAsync(t);
        TempData["Success"] = "Ticket created.";
        return RedirectToAction(nameof(My));
    }
}
