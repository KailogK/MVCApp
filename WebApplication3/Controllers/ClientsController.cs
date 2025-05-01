using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ClientsController : Controller
    {
        private readonly LabDBContext _context;

        public ClientsController(LabDBContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var labDBContext = _context.Clients.Include(c => c.User);
            return View(await labDBContext.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Afm,PhoneNumber,UserId")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", client.UserId);
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", client.UserId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Afm,PhoneNumber,UserId")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", client.UserId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }

        // GET: Client/Home
        public IActionResult Home()
        {
            // Pass the username from session or claims to the view
            var username = HttpContext.Session.GetString("Username") ?? "Client";
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

        [HttpPost]
        public async Task<IActionResult> ViewCallHistory(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch User_id from Users table
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch PhoneNumber from Clients table using User_id
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.UserId);
            if (client == null)
            {
                return NotFound("Client information not found.");
            }

            // Fetch calls using PhoneNumber
            var callHistory = await _context.Calls
                .Where(c => c.PhoneNumber == client.PhoneNumber)
                .Select(c => new
                {
                    c.CallId,
                    c.CallTime,
                    c.Duration
                })
                .ToListAsync();

            return View(callHistory);
        }

        public async Task<IActionResult> ViewBill()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Step 1: Fetch User_id from Users table
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Step 2: Fetch PhoneNumber from Clients table using User_id
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.UserId);
            if (client == null)
            {
                return NotFound("Client information not found.");
            }

            // Step 3: Fetch unpaid bills for the client
            var bills = await _context.Bills
                .Where(b => b.PhoneNumber == client.PhoneNumber && !b.BillPaid)
                .ToListAsync();

            if (!bills.Any())
            {
                ViewBag.Message = "No unpaid bills found.";
                return View();
            }

            // Step 4: Prepare a list of bills with their related calls
            var billDetails = new List<object>();

            foreach (var bill in bills)
            {
                // Fetch calls related to this bill
                var calls = await _context.Calls
                    .Where(c => c.BillId == bill.BillId)
                    .Select(c => new
                    {
                        c.CallId,
                        c.Duration,
                        c.Costs
                    })
                    .ToListAsync();

                // Calculate remaining program price
                decimal totalCallCosts = calls.Sum(c => c.Costs);
                decimal remainingProgramPrice = bill.Costs - totalCallCosts;

                billDetails.Add(new
                {
                    Bill = bill,
                    Calls = calls,
                    RemainingProgramPrice = remainingProgramPrice > 0 ? remainingProgramPrice : 0
                });
            }

            // Pass data to the view
            ViewBag.BillDetails = billDetails;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayBill(int billId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillId == billId);
            if (bill == null)
            {
                return NotFound("Bill not found.");
            }

            // Update calls related to the bill
            var calls = await _context.Calls.Where(c => c.BillId == billId).ToListAsync();
            foreach (var call in calls)
            {
                call.Paid = true;
            }

            // Update the bill
            bill.BillPaid = true;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Bill paid successfully!";
            return RedirectToAction("ViewBill", new { username = User.Identity.Name });
        }

        public async Task<IActionResult> ViewPrograms(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the User ID from the Users table
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find the Phone Number from the Clients table
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.UserId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            // Find the Program assigned to the Phone Number in the Phones table
            var assignedProgram = await _context.Phones
                .Where(p => p.PhoneNumber == client.PhoneNumber)
                .Select(p => new
                {
                    p.ProgramName,
                    p.ProgramEnds
                })
                .FirstOrDefaultAsync();

            // Fetch the list of programs, excluding those with ProgramName = 'Default'
            var programs = await _context.Programs
                .Where(p => p.ProgramName != "Default")
                .Select(p => new
                {
                    p.ProgramName,
                    p.Benefits,
                    p.Charge
                })
                .ToListAsync();

            // Pass the data to the view
            ViewBag.Programs = programs;
            ViewBag.CurrentProgram = assignedProgram;
            ViewBag.Username = username;

            return View();
        }


    }
}
