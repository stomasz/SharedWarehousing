using System.ComponentModel.DataAnnotations;

namespace SharedWarehousingCore.Dtos.IdentityDTOs
{
    public class RegisterDto
    {
        [Required] 
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required] 
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}