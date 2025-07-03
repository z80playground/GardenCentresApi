using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GardenCentresApi.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Region { get; set; }

        public ICollection<GardenCentre> GardenCentres { get; set; } = new List<GardenCentre>();
    }
}