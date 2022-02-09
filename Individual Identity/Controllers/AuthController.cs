using Individual_Identity.Services;
using Individual_Identity.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Individual_Identity.Controllers
{
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authRepository;
        public AuthController(IAuthService authRepository)
        {
            _authRepository = authRepository;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<RestResponse>> Register(Register register)
        {
            return await _authRepository.Register(register);
        }

        /// <summary>
        /// Signin user
        /// </summary>
        /// <param name="signin"></param>
        /// <returns></returns>
        [HttpPost("signin")]
        public async Task<ActionResult<RestResponse>> Signin(Signin signin)
        {
            return await _authRepository.Signin(signin);
        }
    }
}
