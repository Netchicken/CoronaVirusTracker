using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirusTracker.Models;

namespace VirusTracker.Controllers
{
    public class TrackersController : Controller
    {
        private readonly TracingContext _context;

        public TrackersController(TracingContext context)
        {
            _context = context;
        }

        // GET: Trackers
        public async Task<IActionResult> Index()
        {
            var tracingContext = _context.Tracker.Include(t => t.BusinessIdfkNavigation);
            return View(await tracingContext.ToListAsync());
        }

        // GET: Trackers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker
                .Include(t => t.BusinessIdfkNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracker == null)
            {
                return NotFound();
            }

            return View(tracker);
        }

        // GET: Trackers/Create
        public IActionResult Create()
        {
            ViewData["BusinessIdfk"] = new SelectList(_context.Business, "Id", "AdminName");
            return View();
        }

        // POST: Trackers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BusinessIdfk,Name,Phone")] Tracker tracker)
        {
            if (ModelState.IsValid)
            {
                DateTime localDate = DateTime.Now;
                tracker.DateIn = localDate.ToLocalTime();
                tracker.DateOut = localDate.ToLocalTime();

                tracker.Id = Guid.NewGuid();
                _context.Add(tracker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessIdfk"] = new SelectList(_context.Business, "Id", "AdminName", tracker.BusinessIdfk);
            return View(tracker);
        }

        // GET: Trackers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker.FindAsync(id);
            if (tracker == null)
            {
                return NotFound();
            }
            ViewData["BusinessIdfk"] = new SelectList(_context.Business, "Id", "AdminName", tracker.BusinessIdfk);
            return View(tracker);
        }

        // POST: Trackers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,BusinessIdfk,Name,Phone,DateIn,DateOut")] Tracker tracker)
        {
            if (id != tracker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tracker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackerExists(tracker.Id))
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
            ViewData["BusinessIdfk"] = new SelectList(_context.Business, "Id", "AdminName", tracker.BusinessIdfk);
            return View(tracker);
        }

        // GET: Trackers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker
                .Include(t => t.BusinessIdfkNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracker == null)
            {
                return NotFound();
            }

            return View(tracker);
        }

        // POST: Trackers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tracker = await _context.Tracker.FindAsync(id);
            _context.Tracker.Remove(tracker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackerExists(Guid id)
        {
            return _context.Tracker.Any(e => e.Id == id);
        }
    }
}
