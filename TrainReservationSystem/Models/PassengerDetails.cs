using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainReservationSystem.Models
{
    public class PassengerDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        [StringLength(50)]
        public string Gender { get; set; }

        public int PNR { get; set; }

    }
}
