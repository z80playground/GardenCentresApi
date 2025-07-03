using System.ComponentModel.DataAnnotations;

namespace GardenCentresApi.Models
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string Region { get; set; }
    }
}