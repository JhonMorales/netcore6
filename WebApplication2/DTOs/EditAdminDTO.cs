using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs
{
    public class EditAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
