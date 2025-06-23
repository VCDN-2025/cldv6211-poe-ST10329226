using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Booking
        // Modified to include advanced filtering (EventType, Date Range, Venue Availability)
        public async Task<IActionResult> Index(
            string searchString,
            int? eventTypeId,
            DateTime? startDate,
            DateTime? endDate,
            bool? venueAvailability // true for available, false for not available, null for all
        )
        {
            // Start with IQueryable, including necessary navigation properties for filtering and display
            var bookings = _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType) // Include EventType for filtering by EventType
                .AsQueryable();

            // Apply search filter if searchString is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearchString = searchString.ToLower();
                bookings = bookings.Where(b =>
                    b.BookingID.ToLower().Contains(lowerSearchString) ||
                    (b.Event != null && b.Event.Name.ToLower().Contains(lowerSearchString))
                );
            }

            // **NEW FILTER: Filter by Event Type**
            if (eventTypeId.HasValue)
            {
                bookings = bookings.Where(b => b.Event != null && b.Event.EventTypeID == eventTypeId.Value);
            }

            // **NEW FILTER: Filter by Date Range**
            if (startDate.HasValue)
            {
                // Filter bookings that start on or after the specified start date
                bookings = bookings.Where(b => b.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Filter bookings that end on or before the specified end date
                bookings = bookings.Where(b => b.EndDate <= endDate.Value);
            }

            // **NEW FILTER: Filter by Venue Availability**
            if (venueAvailability.HasValue)
            {
                bookings = bookings.Where(b => b.Venue != null && b.Venue.IsAvailable == venueAvailability.Value);
            }

            // Keep ordering by StartDate as before (or you can add more ordering options)
            bookings = bookings.OrderByDescending(b => b.StartDate);

            // Pass current filter values back to the view for pre-filling filter inputs
            ViewData["CurrentSearchString"] = searchString;
            ViewData["CurrentEventTypeId"] = eventTypeId;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-ddTHH:mm"); // Format for datetime-local input
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-ddTHH:mm");     // Format for datetime-local input
            ViewData["CurrentVenueAvailability"] = venueAvailability;

            // Populate dropdown for Event Types for the filter form
            ViewBag.EventTypeIDFilter = new SelectList(await _context.EventTypes.OrderBy(et => et.Name).ToListAsync(), "EventTypeID", "Name", eventTypeId);

            var venueAvailabilityOptions = new List<SelectListItem>
    {
        new SelectListItem { Text = "-- All --", Value = "" }, // Value is empty string for "All"
        new SelectListItem { Text = "Available", Value = "true" },
        new SelectListItem { Text = "Not Available", Value = "false" }
    };
            // The selected value for SelectList constructor needs to match the 'Value' of your SelectListItem
            // Use ToString().ToLower() because the incoming venueAvailability is bool? and the option values are string "true" or "false"
            ViewBag.VenueAvailabilityFilter = new SelectList(venueAvailabilityOptions, "Value", "Text", venueAvailability?.ToString().ToLower());

            // Execute the query and return the results to the view
            return View(await bookings.ToListAsync());
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType) // Include EventType for details display
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            ViewData["VenueID"] = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "Name");
            ViewData["EventID"] = new SelectList(await _context.Events.ToListAsync(), "EventID", "Name");

            var newBooking = new Booking
            {
                StartDate = DateTime.Today.AddDays(1).AddHours(9),
                EndDate = DateTime.Today.AddDays(1).AddHours(17),
                BookingDate = DateTime.Now,
                Status = "Confirmed"
            };
            return View(newBooking);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,VenueID,EventID,StartDate,EndDate,BookingDate,Status")] Booking booking)
        {
            // Re-populate dropdowns for valid scenarios or if model state is invalid
            ViewData["VenueID"] = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "Name", booking.VenueID);
            ViewData["EventID"] = new SelectList(await _context.Events.ToListAsync(), "EventID", "Name", booking.EventID);


            // Manually add model errors for dates and overlapping bookings as before
            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
            }

            var overlappingBookings = await _context.Bookings
                .Where(b => b.VenueID == booking.VenueID &&
                            (booking.StartDate < b.EndDate && booking.EndDate > b.StartDate)
                )
                .ToListAsync();

            if (overlappingBookings.Any())
            {
                var venueName = (await _context.Venues.FindAsync(booking.VenueID))?.Name ?? "Selected Venue";
                ModelState.AddModelError("", $"The {venueName} is already booked during this period. Please choose different dates or a different venue.");
            }

            if (ModelState.IsValid) // Re-check ModelState after custom validation
            {
                if (string.IsNullOrWhiteSpace(booking.BookingID))
                {
                    booking.BookingID = GenerateUniqueBookingId();
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Booking {booking.BookingID} created successfully!";

                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }
            ViewData["VenueID"] = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "Name", booking.VenueID);
            ViewData["EventID"] = new SelectList(await _context.Events.ToListAsync(), "EventID", "Name", booking.EventID);
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BookingID,VenueID,EventID,StartDate,EndDate,BookingDate,Status")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }
            // Re-populate dropdowns for valid scenarios or if model state is invalid
            ViewData["VenueID"] = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "Name", booking.VenueID);
            ViewData["EventID"] = new SelectList(await _context.Events.ToListAsync(), "EventID", "Name", booking.EventID);


            // Manually add model errors for dates and overlapping bookings as before
            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
            }

            var overlappingBookings = await _context.Bookings
                .Where(b => b.VenueID == booking.VenueID &&
                            b.BookingID != booking.BookingID && // EXCLUDE THE CURRENT BOOKING BEING EDITED
                            (
                                (booking.StartDate < b.EndDate && booking.EndDate > b.StartDate)
                            )
                )
                .ToListAsync();

            if (overlappingBookings.Any())
            {
                var venueName = (await _context.Venues.FindAsync(booking.VenueID))?.Name ?? "Selected Venue";
                ModelState.AddModelError("", $"The {venueName} is already booked during this period. Please choose different dates or a different venue.");
            }


            if (ModelState.IsValid) // Re-check ModelState after custom validation
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Booking {booking.BookingID} updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
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
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType) // Include EventType for details display
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Booking {booking.BookingID} deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }

        private string GenerateUniqueBookingId()
        {
            string prefix = "EB";
            string datePart = DateTime.Now.ToString("yyyyMMddHHmmss");
            string randomPart = new Random().Next(100, 999).ToString();
            string bookingId = $"{prefix}-{datePart}-{randomPart}";

            while (_context.Bookings.Any(b => b.BookingID == bookingId))
            {
                randomPart = new Random().Next(100, 999).ToString();
                bookingId = $"{prefix}-{datePart}-{randomPart}";
            }

            return bookingId;
        }
    }
}