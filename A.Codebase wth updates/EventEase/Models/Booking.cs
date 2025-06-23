using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [NotMapped]

namespace EventEase.Models
{
    // *** CHANGE: Renamed from Bookings to Booking ***
    public class Booking
    {
        [Key]
       
        [StringLength(50, ErrorMessage = "Booking ID cannot exceed 50 characters.")]
        [Display(Name = "Booking ID")]
        public string? BookingID { get; set; } // Custom unique ID, e.g., "EB-2025-001"

        [Required(ErrorMessage = "Venue is required.")]
        [Display(Name = "Venue")]
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Event is required.")]
        [Display(Name = "Event")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date and Time")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date and Time")]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now; // Default to current time

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = "Confirmed"; // Default status, can be changed later

        // Navigation properties for related Venue and Event
        [ForeignKey("VenueID")]
        public Venue? Venue { get; set; }

        [ForeignKey("EventID")]
        public Event? Event { get; set; }

        // Helper properties for display (not mapped to DB)
        // These are fine as [NotMapped] if you intend to fill them
        // manually or via a separate ViewModel for specific display purposes.
        // However, for the main Index view, you'll likely use the navigation properties
        // Venue.Name and Event.Name after eager loading, as shown in the controller.
        [NotMapped]
        public string? VenueName { get; set; }
        [NotMapped]
        public string? EventName { get; set; }
    }
}