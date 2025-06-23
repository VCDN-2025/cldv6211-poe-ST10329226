// Models/EventType.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // For ICollection

namespace EventEase.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required(ErrorMessage = "Event Type Name is required.")]
        [StringLength(100, ErrorMessage = "Event Type Name cannot exceed 100 characters.")]
        [Display(Name = "Event Type")]
        public string Name { get; set; }

        // Optional: A description for the event type
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; } // Nullable if description is optional

        // Navigation property for Events of this type
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}