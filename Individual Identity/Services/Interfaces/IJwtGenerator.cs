using Individual_Identity.Core.Domain;

namespace Individual_Identity.Services.Interfaces
{
    public interface IJwtGenerator
    {
        public string CreateToken(User user);
        public string CreateToken(User user, IList<string> roles);
    }
}
