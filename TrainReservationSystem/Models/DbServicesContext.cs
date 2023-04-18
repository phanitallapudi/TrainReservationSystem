using Microsoft.EntityFrameworkCore;

namespace TrainReservationSystem.Models
{
    public class DbServicesContext : DbContext 
    {
        public DbServicesContext(DbContextOptions<DbServicesContext> options) : base(options) 
        {

        }

        public DbSet<BookingHistory> Bookings { get; set; }
        public DbSet<TrainDetails> TrainDetails { get; set; }
        public DbSet<UserProfileDetails> UserProfileDetails { get; set; }
        public DbSet<OlderTrainDetails> OlderTrainDetails { get; set; }

        public DbSet<PassengerDetails> PassengerDetails { get; set; }
    }
}
