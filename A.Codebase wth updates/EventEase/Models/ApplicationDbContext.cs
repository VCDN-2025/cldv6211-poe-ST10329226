using Microsoft.EntityFrameworkCore;
using EventEase.Models; // Keep this line




namespace EventEase.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        //part3 code 
        // --- NEW DbSet FOR EventType ---
        public DbSet<EventType> EventTypes { get; set; }

        // --- END NEW DbSet ---


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BookingViewModel to map to the database view (Needs to be configured)

            // Configure the one-to-many relationship between EventType and Event
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeID)
                .OnDelete(DeleteBehavior.Restrict); // Important: Prevent cascade delete if you want to keep EventTypes even if events are deleted

        }
    }
}