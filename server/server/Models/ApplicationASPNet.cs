using Microsoft.AspNetCore.Identity;

namespace server.Models
{
    public class ApplicationUser : IdentityUser<int> 
    {
        public virtual Doctor? Doctor { get; set; }

        public virtual Patient? Patient { get; set; } 

        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        // public virtual ICollection<ApplicationUserClaim> AspNetUserClaims { get; set; } = new List<ApplicationUserClaim>();

        // public virtual ICollection<ApplicationUserLogin> AspNetUserLogins { get; set; } = new List<ApplicationUserLogin>();

        // public virtual ICollection<ApplicationUserToken> AspNetUserTokens { get; set; } = new List<ApplicationUserToken>();

        public virtual ICollection<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
    }

    public class ApplicationRole : IdentityRole<int>
    {
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
    
    public class ApplicationUserRole : IdentityUserRole<int> 
    {
        public virtual ApplicationUser? User { get; set; }
        public virtual ApplicationRole? Role { get; set; }
    }



    //     public class ApplicationUserClaim : IdentityUserClaim<int>
    // {
    //     public virtual ApplicationUser? User { get; set; }
    // }
    
    // public class ApplicationUserLogin : IdentityUserLogin<int>
    // {
    //     public virtual ApplicationUser? User { get; set; }
    // }
    
    // public class ApplicationRoleClaim : IdentityRoleClaim<int>
    // {
    //     public virtual ApplicationRole? Role { get; set; }
    // }
    
    // public class ApplicationUserToken : IdentityUserToken<int>
    // {
    //     public virtual ApplicationUser? User { get; set; }
    // }
}
