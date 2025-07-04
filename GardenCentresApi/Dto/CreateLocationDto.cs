using System.ComponentModel.DataAnnotations;

namespace GardenCentresApi.Dto
{
    public class CreateLocationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Region { get; set; }
    }
}
