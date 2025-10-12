using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
using NoSQL_project.Repositories.Interfaces;


namespace NoSQL_project.Controllers
{
    public class UserController : Controller 
    {
        private readonly IUserRepository _repo;
        public UserController(IUserRepository repo) => _repo = repo;

        public IActionResult Index()
        {
               List<Users> users = _repo.GetAll();
               return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Users user)
        {
            ModelState.Remove(nameof(Users.Id));
            if (!ModelState.IsValid) 
                return View(user);
            if (string.IsNullOrEmpty(user.Id))
                user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();


            _repo.Add(user);
            TempData["Success"] = "User successfully added!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            var user = _repo.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _repo.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            _repo.Delete(id);
            TempData["Success"] = "User successfully deleted!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ByType(string type)
        {
            var users = _repo.GetByType(type);
            ViewBag.Type = type;
            return View("Index", users);
        }

        public IActionResult ByLocation(string location)
        {
            var users = _repo.GetByLocation(location);
            ViewBag.Location = location;
            return View("Index", users);
        }
    }
}
