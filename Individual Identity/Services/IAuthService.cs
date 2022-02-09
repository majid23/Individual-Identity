using Individual_Identity.Services.Models;

namespace Individual_Identity.Services
{
    public interface IAuthService
    {
        Task<RestResponse> Register(Register register);
        Task<RestResponse> Signin(Signin signin);
    }
}
