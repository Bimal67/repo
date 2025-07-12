using System.ComponentModel.DataAnnotations;

namespace tourly.Models
{
    public class FormSignup
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
