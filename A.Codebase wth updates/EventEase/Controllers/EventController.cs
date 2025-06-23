
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int? eventTypeId)
        {
            // Populate ViewBag for the Event Type dropdown in the view
            ViewBag.EventTypeID = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name");

            IQueryable<Event> events = _context.Events
                                                .Include(e => e.EventType) // Include the related EventType data
                                                .Include(e => e.Venue);    // Include Venue if you display venue info in Events Index

            if (eventTypeId.HasValue && eventTypeId.Value > 0) // Apply filter if a valid EventTypeID is provided
            {
                events = events.Where(e => e.EventTypeID == eventTypeId.Value);
                // Preserve the selected value in the dropdown
                ViewBag.EventTypeID = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name", eventTypeId.Value);
            }

            return View(await events.ToListAsync());
        }

        // ... rest of your EventsController code ...
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                                        .Include(e => e.EventType) // Include EventType for details
                                        .Include(e => e.Venue)     // Include Venue for details
                                        .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewBag.EventTypeID = new SelectList(_context.EventTypes, "EventTypeID", "Name");
            ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "Name"); // Populate for venue dropdown
            return View();
        }
        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Ensure VenueID is included in your bind properties
        public async Task<IActionResult> Create([Bind("Name,Description,EventTypeID,VenueID")] Event @event)
        {
            ModelState.Remove(nameof(@event.Booking));

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }
            // Re-populate ViewBags if model state is invalid
            ViewBag.EventTypeID = new SelectList(_context.EventTypes, "EventTypeID", "Name", @event.EventTypeID);
            ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "Name", @event.VenueID);
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                                        .Include(e => e.Venue) // Include venue to populate dropdown correctly
                                        .Include(e => e.EventType) // Include event type to populate dropdown correctly
                                        .FirstOrDefaultAsync(m => m.EventID == id); // Use FirstOrDefaultAsync with Include

            if (@event == null)
            {
                return NotFound();
            }
            ViewBag.EventTypeID = new SelectList(_context.EventTypes, "EventTypeID", "Name", @event.EventTypeID);
            ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "Name", @event.VenueID); // Populate for venue dropdown
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Ensure EventTypeID is included in your bind properties for edit
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Name,Description,EventTypeID")] Event @event)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
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
            // If model state is invalid, re-populate EventTypes for the dropdown
            // Re-populate ViewBags if model state is invalid
            ViewBag.EventTypeID = new SelectList(_context.EventTypes, "EventTypeID", "Name", @event.EventTypeID);
            ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "Name", @event.VenueID);
            return View(@event);
        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            // Check if there are any associated bookings before showing delete confirmation
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event as it has existing bookings associated with it.";
                return RedirectToAction(nameof(Index)); // Or redirect to Details view, or show error on delete page
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            // Double check for bookings to ensure no race condition allows deletion
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Deletion failed: Event still has associated bookings.";
                return RedirectToAction(nameof(Index));
            }

            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}