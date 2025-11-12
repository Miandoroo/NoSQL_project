using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace NoSQL_project.Controllers
{
    [Authorize(Policy = "ServiceDeskOnly")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _repo;
        public UsersController(IUserRepository repo) => _repo = repo;

        // LIST
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var users = await _repo.GetAllAsync(ct);
            return View(users); // Views/Users/Index.cshtml
        }

        // DETAILS
        [HttpGet]
        public async Task<IActionResult> Details(string id, CancellationToken ct)
        {
            var user = await _repo.GetByIdAsync(id, ct);
            if (user == null) return NotFound();
            return View(user); // Views/Users/Details.cshtml
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create() => View(); // Views/Users/Create.cshtml

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Users user, CancellationToken ct)
        {
            // Genera Id si viene vacío (string con ObjectId)
            if (string.IsNullOrWhiteSpace(user.Id))
                user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            if (!ModelState.IsValid) return View(user);

            await _repo.AddAsync(user, ct);
            TempData["Success"] = "User successfully added!";
            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id, CancellationToken ct)
        {
            var user = await _repo.GetByIdAsync(id, ct);
            if (user == null) return NotFound();
            return View(user); // Views/Users/Edit.cshtml
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Users user, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(user);

            // Garantiza que el Id de ruta prevalece
            user.Id = id;

            await _repo.UpdateAsync(id, user, ct);
            TempData["Success"] = "Successfully edited user";
            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET) - confirmación
        [HttpGet]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var user = await _repo.GetByIdAsync(id, ct);
            if (user == null) return NotFound();
            return View(user); // Views/Users/Delete.cshtml
        }

        // DELETE (POST) - ejecuta borrado
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken ct)
        {
            var user = await _repo.GetByIdAsync(id, ct);
            if (user == null) return NotFound();

            await _repo.DeleteAsync(id, ct);
            TempData["Success"] = "User successfully deleted!";
            return RedirectToAction(nameof(Index));
        }

        // FILTROS (reusan la vista Index)
        [HttpGet]
        public async Task<IActionResult> ByType(string type, CancellationToken ct)
        {
            var users = await _repo.GetByTypeAsync(type, ct);
            ViewBag.Type = type;
            return View("Index", users);
        }

        [HttpGet]
        public async Task<IActionResult> ByLocation(string location, CancellationToken ct)
        {
            var users = await _repo.GetByLocationAsync(location, ct);
            ViewBag.Location = location;
            return View("Index", users);
        }
    }
}
