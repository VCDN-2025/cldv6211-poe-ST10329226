using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Required for ICollection
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(255, ErrorMessage = "Venue name cannot exceed 255 characters.")]
        [Display(Name = "Venue Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters.")]
        public required string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        // ... existing properties like Name, Location, Capacity ...

        [StringLength(2048, ErrorMessage = "Image URL cannot exceed 2048 characters.")]
        [Display(Name = "Image URL")] // This line should already be there and is correct
        public string ImageUrl { get; set; } = "https://via.placeholder.com/600x400?text=No+Image";

        // --- NEW FIELD FOR AVAILABILITY ---
        [Required(ErrorMessage = "Availability status is required.")]
        [Display(Name = "Is Currently Available?")] // This is the correct Display attribute for IsAvailable
        public bool IsAvailable { get; set; } = true; // Default to true
                                                      // --- END NEW FIELD ---

        // ... rest of your Venue model ...

        // Navigation property for related Bookings
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}