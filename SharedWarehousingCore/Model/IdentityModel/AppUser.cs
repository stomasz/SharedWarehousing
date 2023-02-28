using Microsoft.AspNetCore.Identity;
using SharedWarehousingCore.Model.DomainModel;

namespace SharedWarehousingCore.Model.IdentityModel;

    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<RecordHistory> Created { get; set; }
        public ICollection<RecordHistory> Updated { get; set; }
        
    }
