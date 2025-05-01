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
    public class CallsController : Controller
    {
        private readonly LabDBContext _context;

        public CallsController(LabDBContext context)
        {
            _context = context;
        }

        // GET: Calls1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Calls.ToListAsync());
        }

        // GET: Calls1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls
                .FirstOrDefaultAsync(m => m.CallId == id);
            if (call == null)
            {
                return NotFound();
            }

            return View(call);
        }

        // GET: Calls1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Calls1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CallId,PhoneNumber,Costs,ProgramPaid,CallTime,Duration")] Call call)
        {
            if (ModelState.IsValid)
            {
                _context.Add(call);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(call);
        }

        // GET: Calls1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls.FindAsync(id);
            if (call == null)
            {
                return NotFound();
            }
            return View(call);
        }

        // POST: Calls1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CallId,PhoneNumber,Costs,ProgramPaid,CallTime,Duration")] Call call)
        {
            if (id != call.CallId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(call);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CallExists(call.CallId))
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
            return View(call);
        }

        // GET: Calls1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls
                .FirstOrDefaultAsync(m => m.CallId == id);
            if (call == null)
            {
                return NotFound();
            }

            return View(call);
        }

        // POST: Calls1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var call = await _context.Calls.FindAsync(id);
            if (call != null)
            {
                _context.Calls.Remove(call);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CallExists(int id)
        {
            return _context.Calls.Any(e => e.CallId == id);
        }

        // GET: Client/MakeCall
        public IActionResult MakeCall()
        {
            return View();
        }

        // POST: Client/MakeCall
        [HttpPost]
        public async Task<IActionResult> MakeCall(string username, int duration)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Username is required.");
            }

            // Fetch the UserId from the Users table using the provided Username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Get the client's phone number based on the UserId
            var client = _context.Clients.FirstOrDefault(c => c.UserId == user.UserId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            var phoneNumber = client.PhoneNumber;

            // Get the program for the client's phone number
            var phone = _context.Phones.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            if (phone == null)
            {
                return NotFound("Phone not found.");
            }

            //return NotFound(user + "  " + phoneNumber);

            // Check if the client has "Unlimited Calls" program
            var program = _context.Programs.FirstOrDefault(pr => pr.ProgramName == phone.ProgramName);

            // Calculate the cost and determine if the call is paid
            decimal cost = 0.003m * duration;
            bool paid = true;

            if (program == null || program.ProgramName != "Unlimited calls")
            {
                paid = false;
            }
            else
            {
                cost = 0; // No cost for unlimited calls
            }

            // Create a new Call record
            var call = new Call
            {
                PhoneNumber = phoneNumber,
                CallTime = DateTime.Now, // Use the current time as the CallTime
                Duration = duration,
                Costs = cost,
                Paid = paid,
                BillId = -1
            };

            // Save the call to the database
            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            return RedirectToAction("Home", "Clients"); // Redirect back to the Client's home page */
        }
    }
}
