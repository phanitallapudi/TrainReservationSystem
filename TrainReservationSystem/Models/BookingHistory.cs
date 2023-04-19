using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainReservationSystem.Models
{
    public class BookingHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public int PNR { get; set; }
        //[Required]
        //[StringLength(50)]
        //public int UserId { get; set; }
        //[Required]
        //[StringLength(50)]
        //public int TrainId { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]

        public int ticketCount { get; set; }

        public int? TrainId { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("TrainId")]
        public virtual TrainDetails? TrainDetails { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfileDetails? UserProfileDetails { get; set; }

    }
}
