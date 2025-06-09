using System.ComponentModel.DataAnnotations;

namespace jamesmvc.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
