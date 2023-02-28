using System.ComponentModel.DataAnnotations;

namespace SharedWarehousingCore.Dtos.IdentityDTOs
{
    public class LoginDto
    {
        [Required]
        
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}