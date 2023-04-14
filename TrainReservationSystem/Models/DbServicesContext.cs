using Microsoft.EntityFrameworkCore;

namespace TrainReservationSystem.Models
{
    public class DbServicesContext : DbContext 
    {
        public DbSet<BookingHistory> Bookings { get; set; }
        public DbSet<TrainDetails> TrainDetails { get; set; }
        public DbSet<UserProfileDetails> UserProfileDetails { get; set; }

    }
}
