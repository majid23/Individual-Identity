using Individual_Identity.Core.Domain;
using Individual_Identity.Data;
using Individual_Identity.Services.Email;
using Individual_Identity.Services.Interfaces;
using Individual_Identity.Services.Models;
using Microsoft.AspNetCore.Identity;

namespace Individual_Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IEmailSender _emailSender;
        public UserService(IUserAccessor userAccessor,
                UserManager<User> userManager,
                IEmailSender emailSender)
        {
            _userAccessor = userAccessor;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task<RestResponse> ChangePassword(ChangePassword request)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            var result = await _userManager.ChangePasswordAsync(user,
                request.OldPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return RestResponse.SuccessResponse;
            }

            throw new Exception($"Change password is failed.");
        }

        public async Task<RestResponse> RequestCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                string code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.NormalizedEmail.GetHashCode().ToString());

                if (_emailSender.SendCode(user.Email, code))
                {
                    return new RestResponse() { Status = RestResponse.Success, Message = $"The code was sent to email({email})" };
                }
            }

            throw new Exception($"Request code failed.");
        }

        public async Task<RestResponse> ConfirmEmail(string code)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            if (user != null)
            {
                var isValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, code, user.NormalizedEmail.GetHashCode().ToString());

                if (isValid)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded) { return RestResponse.SuccessResponse; }
                }
            }
            throw new Exception($"Confirm email address failed.");
        }

        public async Task<RestResponse> ResetPassword(string email, string newPassword, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var isValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, code, user.NormalizedEmail.GetHashCode().ToString());

                if (isValid)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                    if (result.Succeeded)
                    {
                        return RestResponse.SuccessResponse;
                    }
                }
            }
            throw new Exception($"Reset password failed.");
        }

        public async Task<RestResponse> ChangeEmail(string newEmail)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());
            if (user != null)
            {
                var result = await _userManager.SetEmailAsync(user, newEmail);
                if (result.Succeeded)
                {
                    return new RestResponse()
                    {
                        Status = RestResponse.Success,
                        Message = "Email changed, please confirm it."
                    };
                }
            }
            throw new Exception($"Change email address failed.");
        }
    }
}
