using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainReservationSystem.Models
{
    public class TrainDetails
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string TrainName { get; set; }
        [Required]
        [StringLength(100)]
        public int TrainId { get; set; }
        [Required]
        [StringLength(100)]
        public string Origin { get; set; }
        [Required]
        [StringLength(100)]
        public string Destination { get; set; }
        [Required]
        public DateTime Departure { get; set; }
        [Required]
        public DateTime Arrival { get; set; }
        [Required]
        [StringLength(8)]
        public int SeatCapacity { get; set; }
        [Required]
        [StringLength(8)]
        public int SeatRate { get; set; }


    }
}
