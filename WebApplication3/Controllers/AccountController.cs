using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly LabDBContext _context;

        public AccountController(LabDBContext context)
        {
            _context = context;
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel2 model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return View(model);
            }

            // Hash the password
            using var sha256 = SHA256.Create();
            var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));

            // Create the new user
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                PasswordHash = passwordHash,
                Property = "User" // Set the default role as 'User'
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Registration was successful.";
            return RedirectToAction("Login");
        }


        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username.");
                return View(model);
            }

            // Verify the password
            using var sha256 = SHA256.Create();
            var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));

            if (user.PasswordHash != passwordHash)
            {
                ModelState.AddModelError("", "Invalid password.");
                return View(model);
            }

            HttpContext.Session.SetString("Username", user.Username);
            //TempData["Username"] = user.Username;

            // Redirect based on user property
            switch (user.Property.ToLower())
            {
                case "admin":
                    return RedirectToAction("Home", "Admins");
                case "seller":
                    return RedirectToAction("Home", "Sellers");
                case "client":
                    return RedirectToAction("Home", "Clients");
                case "user":
                    return RedirectToAction("Home", "Users");
                default:
                    ModelState.AddModelError("", "User role not recognized.");
                    return View(model);
            }
        }



        // Logout (for session-based authentication)
        public async Task<IActionResult> Logout()
        {
            // Clear the user session (or authentication cookies if used)
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
