using Individual_Identity.Services.Models;

namespace Individual_Identity.Services
{
    public interface IUserService
    {
        Task<RestResponse> ChangePassword(ChangePassword request);
        Task<RestResponse> RequestCode(string email);
        Task<RestResponse> ConfirmEmail(string code);
        Task<RestResponse> ResetPassword(string email, string newPassword, string code);
        Task<RestResponse> ChangeEmail(string newEmail);
    }
}
