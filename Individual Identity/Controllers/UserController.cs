using Individual_Identity.Services;
using Individual_Identity.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Individual_Identity.Controllers
{
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Change password when user signin.
        /// </summary>
        /// <param name="changePassword">Enter old password and new password</param>
        /// <returns></returns>
        [HttpPost("changePassword", Name = "ChangePassword")]
        public async Task<ActionResult<RestResponse>> ChangePassword(ChangePassword changePassword)
        {
            return await _userService.ChangePassword(changePassword);
        }


        /// <summary>
        /// Request code by email when you want confirm email or reset password.
        /// </summary>
        /// <param name="email">Set the email where you want to receive the code</param>
        /// <returns></returns>
        [HttpGet("requestCode", Name = "RequestCode")]
        [AllowAnonymous]
        public async Task<ActionResult<RestResponse>> RequestCode(string email)
        {
            return await _userService.RequestCode(email);
        }

        /// <summary>
        /// Confirm the email with the code you received from the request code
        /// </summary>
        /// <param name="code">Set the code you received via email</param>
        /// <returns></returns>
        [HttpGet("confirmEmail", Name = "ConfirmEmail")]
        public async Task<ActionResult<RestResponse>> ConfirmEmail(string code)
        {
            return await _userService.ConfirmEmail(code);
        }

        /// <summary>
        /// Use reset password when user forgot it
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="newPassword">New password</param>
        /// <param name="code">Set the code you received via email</param>
        /// <returns></returns>
        [HttpGet("resetPassword", Name = "ResetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<RestResponse>> ResetPassword(string email, string newPassword, string code)
        {
            return await _userService.ResetPassword(email, newPassword, code);
        }


        /// <summary>
        /// Change user's email without confirmation
        /// </summary>
        /// <param name="newEmail">New user's email</param>
        /// <returns></returns>
        [HttpGet("changeEmail", Name = "ChangeEmail")]
        public async Task<ActionResult<RestResponse>> ChangeEmail(string newEmail)
        {
            return await _userService.ChangeEmail(newEmail);
        }
    }
}
