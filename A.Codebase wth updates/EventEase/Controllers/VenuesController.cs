using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System; // Added for Guid and Path for potential image upload logic
// If you uncomment the BlobStorage code later, you'll need this:
// using Azure.Storage.Blobs;
// using System.IO; // For Path.GetExtension and stream operations

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        // You'll need this for Azure Blob Storage integration (Part 2)
        // Uncomment this line and the parameter in the constructor when implementing image uploads
        // private readonly Azure.Storage.Blobs.BlobServiceClient _blobServiceClient;

        public VenuesController(ApplicationDbContext context /*, Azure.Storage.Blobs.BlobServiceClient blobServiceClient = null */)
        {
            _context = context;
            // Initialize _blobServiceClient here if you uncomment it in the constructor
            // _blobServiceClient = blobServiceClient;
        }

        // GET: Venues
        public async Task<IActionResult> Index(bool? isAvailable) // Nullable bool for optional filter
        {
            IQueryable<Venue> venues = _context.Venues;

            if (isAvailable.HasValue) // Apply filter only if isAvailable is provided
            {
                venues = venues.Where(v => v.IsAvailable == isAvailable.Value);
            }

            // Pass the current filter selection to the view for checkbox state
            ViewBag.IsAvailableFilter = isAvailable;

            return View(await venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Make sure IsAvailable is included in your bind properties
        public async Task<IActionResult> Create([Bind("Name,Location,Capacity,ImageUrl,IsAvailable")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // VenueID is kept in [Bind] here because it's used to identify the record for update
        // For Part 2, you will also add an IFormFile parameter here for file uploads (e.g., IFormFile imageFile).
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,Name,Location,Capacity,ImageUrl")] Venue venue /*, IFormFile imageFile */)
        {
            if (id != venue.VenueID)
            {
                return NotFound();
            }

            // For Part 2, similar image upload/deletion logic will go here
            /*
            // Check if a new image was uploaded during edit
            if (imageFile != null && imageFile.Length > 0)
            {
                // Ensure _blobServiceClient is injected and not null
                // if (_blobServiceClient == null)
                // {
                //     ModelState.AddModelError("", "Azure Blob Storage service is not configured correctly. Image upload failed.");
                //     return View(venue);
                // }

                var containerClient = _blobServiceClient.GetBlobContainerClient("eventimages");
                await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                // Fetch the old venue to get the old ImageUrl before updating
                // Use AsNoTracking() to avoid conflicting with the 'venue' entity being updated
                var oldVenue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueID == venue.VenueID);

                // Delete old image if it exists and is different from the new one
                if (oldVenue != null && !string.IsNullOrEmpty(oldVenue.ImageUrl) && oldVenue.ImageUrl != venue.ImageUrl)
                {
                    try
                    {
                        Uri uri = new Uri(oldVenue.ImageUrl);
                        string blobName = Path.GetFileName(uri.LocalPath);
                        var oldBlobClient = containerClient.GetBlobClient(blobName);
                        await oldBlobClient.DeleteIfExistsAsync(); // Delete the old blob
                    }
                    catch (Exception ex)
                    {
                        // Log the error but don't prevent the update if old image deletion fails
                        Console.WriteLine($"Error deleting old blob: {ex.Message}");
                    }
                }

                // Upload the new image
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var blobClient = containerClient.GetBlobClient(uniqueFileName);
                using (var stream = imageFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }
                venue.ImageUrl = blobClient.Uri.ToString(); // Store the new blob URL
            }
            */

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            // Check if there are any associated bookings before showing delete confirmation (Part 2 Requirement)
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue as it has existing bookings associated with it.";
                // Redirect back to details to show the error message on the venue's page
                return RedirectToAction(nameof(Details), new { id = id });
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            // Double check for bookings to ensure no race condition allows deletion (Part 2 Requirement)
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Deletion failed: Venue still has associated bookings.";
                // If deletion fails due to bookings, redirect to details page to show the error
                return RedirectToAction(nameof(Details), new { id = id });
            }

            if (venue != null)
            {
     
                

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueID == id);
        }
    }
}