using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace jamesmvc.Models
{
    public class DriverProfile
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } // 對應 IdentityUser.Id

        [MaxLength(100)]
        public string? ServiceCity { get; set; }


        [MaxLength(100)]
        public string? ServiceDistrict { get; set; }

        // 關聯到 IdentityUser
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }


        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

    }
}