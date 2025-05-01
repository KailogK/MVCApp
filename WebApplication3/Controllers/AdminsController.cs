using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class AdminsController : Controller
    {
        private readonly LabDBContext _context;

        public AdminsController(LabDBContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            var labDBContext = _context.Admins.Include(a => a.User);
            return View(await labDBContext.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,UserId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", admin.UserId);
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", admin.UserId);
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,UserId")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", admin.UserId);
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }

        // GET: Admin/Home
        public IActionResult Home()
        {
            // Pass the username from session or claims to the view
            var username = HttpContext.Session.GetString("Username") ?? "Admin";
            ViewBag.Username = username;
            return View();
        }

        // Logout action
        public IActionResult Logout()
        {
            // Clear session or authentication cookie
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // GET: Admins/CreateSeller
        public async Task<IActionResult> CreateSeller()
        {
            // Fetch all users with the property 'user' or 'client'
            var users = await _context.Users
                .Where(u => u.Property == "user" || u.Property == "client")
                .Select(u => new SelectListItem
                {
                   Value = u.UserId.ToString(),
                    Text = u.Username
                })
               .ToListAsync();

               var model = new CreateSellerViewModel
                {
                    UsersList = users
              };

            return View(model);
        }

        // POST: Admins/CreateSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSeller(int userId)
        {
            // Find the user by ID
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction(nameof(CreateSeller));
            }

            // Add the user to the Sellers table
            var newSeller = new Seller
            {
                UserId = user.UserId,
                // Add additional seller-specific fields here if needed
            };
            _context.Sellers.Add(newSeller);

            // Update the user's property to 'seller'
            user.Property = "Seller";

            await _context.SaveChangesAsync();

            TempData["Message"] = $"{user.Username} has been successfully promoted to Seller.";
            return RedirectToAction(nameof(CreateSeller));
        }

        public async Task<IActionResult> RemoveSeller()
        {
            // Retrieve a list of all sellers
            var sellers = await _context.Sellers
                .Include(s => s.User)  // Include User information (like username)
                .ToListAsync();

            return View(sellers);  // Pass sellers to the view
        }

        // POST: Admins/RemoveSeller/5 (Remove a seller)
        [HttpPost]
        public async Task<IActionResult> RemoveSeller(int id)
        {
            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }

            // Remove seller from the Sellers table
            _context.Sellers.Remove(seller);

            // Check if the user is also in the Clients table
            var user = await _context.Users.FindAsync(seller.UserId);
            if (user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                if (client != null)
                {
                    // Change the user's property to Client
                    user.Property = "Client";
                }
                else
                {
                    // Demote user to "User"
                    user.Property = "User";
                }
                _context.Users.Update(user);
            }

            // Save changes
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{user.Username} has been successfully Demoted from Seller.";
            return RedirectToAction(nameof(RemoveSeller));
        }

        // GET: CreateProgram
        public IActionResult CreateProgram()
        {
            return View();
        }

        // POST: CreateProgram
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgram(CreateProgramViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create the new program
            var program = new Programme
            {
                ProgramName = model.ProgramName,
                Benefits = model.Benefits,
                Charge = model.Charge
            };

            // Add the program to the Programs table
            _context.Programs.Add(program);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{program.ProgramName} has been successfully Created";
            return RedirectToAction(nameof(CreateProgram)); // Redirect to a list of programs, for example
        }

        public IActionResult EditProgram()
        {
            // Fetch all programs to populate the dropdown list
            var programs = _context.Programs.ToList();

            ViewBag.Programs = programs; // Pass the list to the view

            return View();
        }





        [HttpPost]
        public async Task<IActionResult> EditProgram(string selectedProgramName, string programName, string benefits, decimal charge)
        {
            if (string.IsNullOrEmpty(selectedProgramName))
            {
                TempData["Message"] = "Please select a program to edit.";
                return RedirectToAction(nameof(EditProgram));
            }

            // Start a transaction for consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Find the program by name
                var program = await _context.Programs.FirstOrDefaultAsync(p => p.ProgramName == selectedProgramName);
                if (program == null)
                {
                    TempData["Message"] = "Program not found.";
                    return RedirectToAction(nameof(EditProgram));
                }

                // Update the foreign key in the Phones table
                var phonesToUpdate = await _context.Phones
                    .Where(p => p.ProgramName == selectedProgramName)
                    .ToListAsync();

                foreach (var phone in phonesToUpdate)
                {
                    phone.ProgramName = programName; // Update the FK to the new value
                    _context.Phones.Update(phone);
                }

                await _context.SaveChangesAsync(); // Save phones table updates first

                // Update the primary key in the Programs table
                // program.ProgramName = programName; // Update the PK
                program.Benefits = benefits;
                program.Charge = charge;

                _context.Programs.Update(program); // Update program details
                await _context.SaveChangesAsync(); // Save program updates

                // Commit the transaction
                await transaction.CommitAsync();

                TempData["Message"] = $"{selectedProgramName} has been successfully updated!";
                return RedirectToAction(nameof(EditProgram));
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                await transaction.RollbackAsync();
                TempData["Message"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(EditProgram));
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetProgrammeDetails(string programName)
        {
            var programme = await _context.Programs
                .FirstOrDefaultAsync(p => p.ProgramName == programName);

            if (programme == null)
            {
                return NotFound();
            }

            return Json(new
            {
                programName = programme.ProgramName,
                benefits = programme.Benefits,
                charge = programme.Charge
            });
        }


    }
}
