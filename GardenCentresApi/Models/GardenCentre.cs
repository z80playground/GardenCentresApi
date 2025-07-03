using System.ComponentModel.DataAnnotations;

namespace GardenCentresApi.Models
{
    public class GardenCentre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public string Region { get; set; }

        public Location Location { get; set; }
    }
}