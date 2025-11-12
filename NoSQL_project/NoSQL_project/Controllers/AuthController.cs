using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_project.Services.Interfaces;
using System.Security.Claims;

namespace NoSQL_project.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _users;
        public AuthController(IUserService users) => _users = users;

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            // 1) Buscar usuario
            var user = await _users.GetByUsernameAsync(username);
            if (user == null || string.IsNullOrWhiteSpace(user.password) ||
                !BCrypt.Net.BCrypt.Verify(password, user.password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            // 2) Normalizar rol (fallback a RegularEmployee)
            var role = (user.Role ?? "RegularEmployee").Trim();
            role = role.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" :
                   role.Equals("servicedesk", StringComparison.OrdinalIgnoreCase) ? "ServiceDesk" :
                   "RegularEmployee";

            // 3) Claims para la cookie (usa el rol normalizado)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username ?? $"{user.FirstName} {user.LastName}".Trim()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, role),                   // 👈 MUY IMPORTANTE
                new Claim("username", user.Username ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 4) Iniciar sesión (opcional: persistente)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            // 5) Redirección
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return role switch
            {
                "Admin" => RedirectToAction("Index", "Users"),
                "ServiceDesk" => RedirectToAction("Index", "Users"),
                _ => RedirectToAction("My", "Ticket")
            };
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Denied() => View();

    }
}
