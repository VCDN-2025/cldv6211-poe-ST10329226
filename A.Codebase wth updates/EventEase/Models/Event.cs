using System.Collections.Generic; // Required for ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey attribute
namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(255, ErrorMessage = "Event name cannot exceed 255 characters.")]
        public string? Name { get; set; }

        [Display(Name = "Description (Optional)")]
        public string? Description { get; set; }

        // --- NEW FIELDS FOR EVENT TYPE ---
        [Display(Name = "Event Type")]
        public int EventTypeID { get; set; } // Foreign key to EventType

        [ForeignKey("EventTypeID")]
        public EventType? EventType { get; set; } // Navigation property, nullable as it might not be loaded initially
                                                  // --- END NEW FIELDS ---

        // --- NEW FIELDS FOR VENUE (ADD THESE!) ---
        [Display(Name = "Venue")]
        [Required(ErrorMessage = "A venue is required for the event.")] // Make Venue mandatory
        public int VenueID { get; set; } // Foreign key to Venue

        [ForeignKey("VenueID")]
        public Venue? Venue { get; set; } // Navigation property

        // --- END NEW FIELDS ---

        // Navigation property for related Bookings
        public ICollection<Booking> Booking { get; set; } = new List<Booking>();
    }
}