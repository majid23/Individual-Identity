using Microsoft.AspNetCore.Identity;

namespace Individual_Identity.Core.Domain
{
    public class User : IdentityUser<int>, ISoftDeletedEntity
    {
        public DateTime BirthDate { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public bool Deleted { get; set; }
    }
}
