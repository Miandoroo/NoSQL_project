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
            try
            {
                List<Users> users = _repo.GetAll();
                return View(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }
    }
}
