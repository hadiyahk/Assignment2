using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    public class FansController : Controller
    {
        private readonly SportsDbContext _context;

        public FansController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: Fans
        public async Task<IActionResult> Index()
        {
            var fans = await _context.Fans
                .Include(f => f.Subscriptions)
                .ToListAsync();

            var model = fans.Select(fan => new Fan
            {
                Id = fan.Id,
                LastName = fan.LastName,
                FirstName = fan.FirstName,
                BirthDate = fan.BirthDate,
                Subscriptions = fan.Subscriptions.ToList()
            }).ToList();

            return View(model);
        }


        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans.FindAsync(id);
            if (fan == null)
            {
                return NotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.Id))
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
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fan = await _context.Fans.FindAsync(id);
            if (fan != null)
            {
                _context.Fans.Remove(fan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FanExists(int id)
        {
            return _context.Fans.Any(e => e.Id == id);
        }



        public IActionResult Subscriptions(int id)
        {
            // Fetch the fan and related subscriptions
            var fan = _context.Fans
                .Include(f => f.Subscriptions)
                .ThenInclude(s => s.SportClub)
                .FirstOrDefault(f => f.Id == id);

            if (fan == null)
            {
                return NotFound();
            }

            // Fetch all sport clubs into memory
            var allSportClubs = _context.SportClubs.ToList();

            // Prepare the view model
            var model = new FanSubscriptionViewModel
            {
                Fan = fan,
                Subscriptions = allSportClubs
                    .Select(sc => new SportClubSubscriptionViewModel
                    {
                        SportClubId = sc.Id,
                        Title = sc.Title,
                        IsMember = fan.Subscriptions.Any(s => s.SportClubId == sc.Id)
                    })
                    .OrderBy(sc => sc.IsMember ? 0 : 1)
                    .ThenBy(sc => sc.Title)
                    .ToList()
            };

            return View(fan);
        }

        public async Task<IActionResult> EditSubscriptions(int id)
        {
            var fan = await _context.Fans
                .Include(f => f.Subscriptions)
                .ThenInclude(s => s.SportClub)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fan == null)
            {
                return NotFound();
            }

            // Retrieve all sport clubs
            var sportClubs = await _context.SportClubs.ToListAsync();

            // Create a dictionary to track whether a sport club is subscribed to
            var sportClubDictionary = sportClubs.ToDictionary(sc => sc.Id, sc => sc);

            // Map subscriptions to view model and sort
            var subscriptions = sportClubs.Select(sc => new SportClubSubscriptionViewModel
            {
                SportClubId = sc.Id,
                Title = sc.Title,
                IsMember = fan.Subscriptions.Any(s => s.SportClubId == sc.Id)
            }).ToList();

            // Sort: subscribed clubs first, then the rest alphabetically
            subscriptions = subscriptions
                .OrderByDescending(sc => sc.IsMember) // Subscribed clubs first
                .ThenBy(sc => sc.Title) // Alphabetical order
                .ToList();

            return View(subscriptions);
        }


        [HttpPost]
        public async Task<IActionResult> EditSubscriptions(int id, List<string> subscriptions)
        {
            var fan = await _context.Fans
                .Include(f => f.Subscriptions)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fan == null)
            {
                return NotFound();
            }

            // Convert the list of subscription IDs to a HashSet
            var newSubscriptions = new HashSet<string>(subscriptions);

            // Identify subscriptions to add and remove
            var currentSubscriptions = fan.Subscriptions.Select(s => s.SportClubId).ToHashSet();
            var toAdd = newSubscriptions.Except(currentSubscriptions).ToList();
            var toRemove = currentSubscriptions.Except(newSubscriptions).ToList();

            // Add new subscriptions
            foreach (var sportClubId in toAdd)
            {
                fan.Subscriptions.Add(new Subscription
                {
                    FanId = fan.Id,
                    SportClubId = sportClubId
                });
            }

            // Remove old subscriptions
            foreach (var sportClubId in toRemove)
            {
                var subscriptionToRemove = fan.Subscriptions
                    .FirstOrDefault(s => s.SportClubId == sportClubId);
                if (subscriptionToRemove != null)
                {
                    _context.Subscriptions.Remove(subscriptionToRemove);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewSubscription(int id)
        {
            // Fetch the fan with subscriptions
            var fan = await _context.Fans
                .Include(f => f.Subscriptions)
                .ThenInclude(s => s.SportClub)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fan == null)
            {
                return NotFound();
            }

            // Create the view model
            var viewModel = new FanSubscriptionViewModel
            {
                Fan = fan,
                FanId = fan.Id,
                Subscriptions = fan.Subscriptions
                    .Select(s => new SportClubSubscriptionViewModel
                    {
                        SportClubId = s.SportClubId,
                        Title = s.SportClub.Title, // Ensure that the Title property exists
                        IsMember = true // or set based on any logic you have
                    }).ToList()
            };

            return View(viewModel);
        }





    }
}
