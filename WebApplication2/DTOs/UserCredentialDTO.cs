using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs
{
    public class UserCredentialDTO
    {
        [Required]
        [EmailAddress]
        public string Email{ get; set; }
        [Required]
        public string Password { get; set; }
    }
}
