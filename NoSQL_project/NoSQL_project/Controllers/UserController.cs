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



        [HttpGet]
        public IActionResult Update(string id)
        {
            Users user = _repo.GetById(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Update(string id, Users user)
        {


            if (!ModelState.IsValid)
            {
                return View(user);
            }
            _repo.Update(id, user);
            TempData["Success"] = "Successfully edited user";
            return RedirectToAction(nameof(Index));
        }
    }
}
