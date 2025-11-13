using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Models;
using NoSQL_project.Models.ViewModels;
using NoSQL_project.Services.Interfaces;
using System.Security.Claims;

namespace NoSQL_project.Controllers
{
    public class UserController : Controller 
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userService.AuthenticateUser(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var claims = _userService.CreateClaims(user);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!_userService.ValidateUserRegistration(model.Username, model.Email, out string? errorMessage))
            {
                if (errorMessage.Contains("Username"))
                    ModelState.AddModelError("Username", errorMessage);
                else
                    ModelState.AddModelError("Email", errorMessage);
                return View(model);
            }

            var newUser = _userService.CreateUserFromRegister(model);

            try
            {
                _userService.Create(newUser, model.Password);
                TempData["Success"] = "Account created successfully! You can now login.";
                return RedirectToAction("Login", "User");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        public IActionResult Index()
        {
            return View(_userService.GetAll());
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user, string password)
        {
            ModelState.Remove(nameof(NoSQL_project.Models.User.Id));
            ModelState.Remove(nameof(NoSQL_project.Models.User.PasswordHash));
            if (!ModelState.IsValid) 
                return View(user);

            try
            {
                _userService.Create(user, password);
                TempData["Success"] = "User successfully added!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        public IActionResult Details(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            return View(user);
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        [HttpGet]
        public IActionResult Update(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            return View(user);
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string id, User user, string password)
        {
            ModelState.Remove(nameof(NoSQL_project.Models.User.PasswordHash));
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                _userService.Update(id, user, password);
                TempData["Success"] = "Successfully edited user";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            return View(user);
        }

        [Authorize(Roles = "ServiceDeskEmployee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _userService.Delete(id);
                TempData["Success"] = "User successfully deleted!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}
