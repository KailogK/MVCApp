using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class SellersController : Controller
    {
        private readonly LabDBContext _context;

        public SellersController(LabDBContext context)
        {
            _context = context;
        }

        // GET: Sellers
        public async Task<IActionResult> Index()
        {
            var labDBContext = _context.Sellers.Include(s => s.User);
            return View(await labDBContext.ToListAsync());
        }

        // GET: Sellers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // GET: Sellers/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SellerId,UserId")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", seller.UserId);
            return View(seller);
        }

        // GET: Sellers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", seller.UserId);
            return View(seller);
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SellerId,UserId")] Seller seller)
        {
            if (id != seller.SellerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seller);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerExists(seller.SellerId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", seller.UserId);
            return View(seller);
        }

        // GET: Sellers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seller = await _context.Sellers.FindAsync(id);
            if (seller != null)
            {
                _context.Sellers.Remove(seller);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerExists(int id)
        {
            return _context.Sellers.Any(e => e.SellerId == id);
        }

        // GET: Seller/Home
        public IActionResult Home()
        {
            // Pass the username from session or claims to the view
            var username = HttpContext.Session.GetString("Username") ?? "Seller";
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

        public IActionResult RegisterClient()
        {
            // Redirect to the page for registering a new client
            return View();
        }

        public IActionResult IssueBill()
        {
            var phoneNumbers = _context.Phones.Select(p => p.PhoneNumber).ToList();
            return View(phoneNumbers); // Pass the model (list of phone numbers)
        }


        [HttpPost]
        public async Task<IActionResult> IssueBill(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return BadRequest("Phone number is required.");
            }

            // Step 1: Check if the phone exists
            var phone = await _context.Phones.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
            if (phone == null)
            {
                return NotFound($"Phone number {phoneNumber} not found.");
            }

            // Step 2: Check if a bill is necessary
            bool hasUnpaidCalls = await _context.Calls.AnyAsync(c => c.PhoneNumber == phoneNumber && !c.Paid);
            bool isProgramUnpaid = !phone.ProgramPaid;

            if (!hasUnpaidCalls && !isProgramUnpaid)
            {
                TempData["Message"] = "Client has already paid everything!";
                return RedirectToAction(nameof(IssueBill)); // Adjust to the correct view
            }

            // Step 3: Create a new bill
            var newBill = new Bill
            {
                PhoneNumber = phoneNumber,
                Costs = 0, // Initial cost
                BillPaid = false
            };
            _context.Bills.Add(newBill);
            await _context.SaveChangesAsync();

            // Step 4: Associate unpaid calls with the new bill
            var unpaidCalls = await _context.Calls
                .Where(c => c.PhoneNumber == phoneNumber && !c.Paid && c.BillId == -1)
                .ToListAsync();

            foreach (var call in unpaidCalls)
            {
                call.BillId = newBill.BillId; // Assign the new bill ID
            }

            // Step 5: Calculate total costs
            decimal totalCosts = unpaidCalls.Sum(c => c.Costs);

            if (isProgramUnpaid)
            {
                // Fetch the program cost dynamically from the Programs table
                var programCost = await _context.Programs
                    .Where(pr => pr.ProgramName == phone.ProgramName)
                    .Select(pr => pr.Charge) // Assuming 'Charge' is the program's cost column
                    .FirstOrDefaultAsync();

                totalCosts += programCost;

                // Update the phone record to mark the program as paid                                      //
                phone.ProgramPaid = true;                                                                   // TO CHANGE -> когато се генерира нова сметка, програмата се "плаща", тоест става платена = true. 
                phone.ProgramEnds = DateTime.Now.AddMonths(1); // Extend the program for another month      //
            }

            newBill.Costs = totalCosts;

            if (totalCosts == 0)
            {
                TempData["Message"] = "Client doesn't owe anything atm. However, they still have bills left to pay";
                return RedirectToAction(nameof(IssueBill)); // Adjust to the correct view
            }

            // Step 6: Save all changes
            await _context.SaveChangesAsync();

            TempData["Message"] = $"A new bill has been issued for phone number {phoneNumber}.";
            return RedirectToAction(nameof(IssueBill)); // Adjust to the correct view
        }



        // GET: ChangeClientProgram
        public IActionResult ChangeClientProgram()
        {
            // Fetch all phone records with their current programs
            var phonePrograms = _context.Phones
                .Join(
                    _context.Programs,
                    phone => phone.ProgramName, // Key from Phones table
                    program => program.ProgramName, // Key from Programs table
                    (phone, program) => new
                    {
                        phone.PhoneNumber,
                        program.ProgramName,
                        program.Benefits,
                        program.Charge
                    }
                )
                .ToList();


            // Fetch all programs
            var programs = _context.Programs
                .Select(p => new { p.ProgramName, p.Benefits, p.Charge })
                .ToList();

            // Pass data to the view
            ViewBag.PhoneRecords = phonePrograms;
            ViewBag.Programs = programs;

            ViewBag.SelectedPhoneNumber = TempData["SelectedPhoneNumber"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeClientProgram(string phoneNumber, string programName)
        {
            // Find the phone record based on the phone number
            var phoneEntry = await _context.Phones
                .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);

            if (phoneEntry == null)
            {
                return NotFound($"Phone number {phoneNumber} not found.");
            }

            // Update the data for the selected phone number
            phoneEntry.ProgramName = programName;
            phoneEntry.ProgramEnds = DateTime.Now.AddMonths(1);
            phoneEntry.ProgramPaid = false;

            await _context.SaveChangesAsync();

            // Set a success message
            TempData["Message"] = $"Program '{programName}' has been successfully assigned to phone number '{phoneNumber}'.";

            return RedirectToAction(nameof(ChangeClientProgram));
        }


        // GET: PromoteUserToClient
        public IActionResult PromoteUserToClient()
        {
            // Fetch users eligible for promotion (Property == "User")
            var users = _context.Users
                .Where(u => u.Property == "User")
                .ToList();

            ViewBag.Users = users; // Pass users to the view
            return View();
        }

        // POST: PromoteUserToClient
        [HttpPost]
        public async Task<IActionResult> PromoteUserToClient(int userId, string afm, string phoneNumber)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null || user.Property != "User")
            {
                return NotFound("User not found or not eligible for promotion.");
            }

            // Check if the phone number already exists in the Phones table
            var existingPhone = await _context.Phones
                .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);

            if (existingPhone != null)
            {
                TempData["ErrorMessage"] = $"The phone number {phoneNumber} is already in use.";
                return RedirectToAction("PromoteUserToClient");
            }

            // Promote to Client
            user.Property = "Client";

            // Add to Clients table
            var client = new Client
            {
                UserId = userId,
                Afm = afm,
                PhoneNumber = phoneNumber
            };

            _context.Clients.Add(client);

            // Add to Phones table
            var phone = new Phone
            {
                PhoneNumber = phoneNumber,
                ProgramName = "Default",
                ProgramPaid = true
            };

            _context.Phones.Add(phone);

            await _context.SaveChangesAsync();

            TempData["Message"] = $"User has been successfully promoted to Client.";
            return RedirectToAction("PromoteUserToClient");
        }



        // GET: CreateNewClient
        public IActionResult CreateNewClient()
        {
            return View();
        }

        // POST: CreateNewClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewClient(RegisterViewModel model)
        {
            Console.WriteLine("POST: CreateNewClient triggered"); // Debug log

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model validation failed");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
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

            // Check if the phone number already exists
            if (_context.Clients.Any(c => c.PhoneNumber == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "The phone number already exists.");
                return View(model);
            }

            // Hash the password
            using var sha256 = SHA256.Create();
            var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));

            // Add to Users table
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                PasswordHash = passwordHash,
                Property = "Client"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Save to get UserId

            // Add to Clients table
            var client = new Client
            {
                UserId = user.UserId,
                Afm = model.AFM,
                PhoneNumber = model.PhoneNumber
            };

            _context.Clients.Add(client);

            // Add to Phones table
            var phone = new Phone
            {
                PhoneNumber = model.PhoneNumber,
                ProgramName = "Default",
                ProgramPaid = true
            };

            _context.Phones.Add(phone);

            await _context.SaveChangesAsync();
            TempData["Message"] = $"Client has been successfully registered.";

            return RedirectToAction("Home");
        }

        // GET: DemoteClientToUser
        public IActionResult DemoteClientToUser()
        {
            // Fetch clients eligible for demotion
            var clients = _context.Clients
                .Include(c => c.User) // Include User details
                .ToList();

            ViewBag.Clients = clients; // Pass clients to the view
            return View();
        }

        // POST: DemoteClientToUser
        [HttpPost]
        public async Task<IActionResult> DemoteClientToUser(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.User) // Include User for easier access
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client == null)
            {
                return NotFound("Client not found.");
            }

            // Update the user's property to "User"
            var user = client.User;
            user.Property = "User";

            // Remove the client entry
            _context.Clients.Remove(client);

            await _context.SaveChangesAsync();

            return RedirectToAction("DemoteClientToUser");
        }

    }
}
