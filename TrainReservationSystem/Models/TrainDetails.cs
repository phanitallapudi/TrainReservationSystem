using System.ComponentModel.DataAnnotations;

namespace TrainReservationSystem.Models
{
    public class TrainDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TrainName { get; set; }
        [Required]
        public string TrainNumber { get; set; }
        [Required]
        public string origin { get; set; }
        [Required]
        public string destination { get; set; }
        [Required]
        public int SeatCount { get; set; }



    }
}
