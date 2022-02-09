using Microsoft.AspNetCore.Identity;

namespace Individual_Identity.Core.Domain
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
