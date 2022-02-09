using AutoMapper;
using Individual_Identity.Core.Domain;
using Individual_Identity.Services.Errors;
using Individual_Identity.Services.Interfaces;
using Individual_Identity.Services.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Individual_Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<User> userManager,
            IJwtGenerator jwtGenerator,
            SignInManager<User> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
            _signInManager = signInManager;
            _mapper = mapper;

        }

        public async Task<RestResponse> Register(Register register)
        {
            if (string.IsNullOrEmpty(register.Email) && string.IsNullOrEmpty(register.Username))
                throw new RestException(HttpStatusCode.BadRequest, "Please enter email or username or both");

            if (!string.IsNullOrEmpty(register.Email) && !CommonHelper.IsValidEmail(register.Email))
                throw new RestException(HttpStatusCode.BadRequest, "Email is not valid.");

            if (!string.IsNullOrEmpty(register.Email) && _userManager.Users.Where(x => x.NormalizedEmail == register.Email.ToUpper()).Any())
                throw new RestException(HttpStatusCode.BadRequest, "Email already exists");

            if (!string.IsNullOrEmpty(register.Username) && _userManager.Users.Where(x => x.NormalizedUserName == register.Username.ToUpper()).Any())
                throw new RestException(HttpStatusCode.BadRequest, "Username already exists");

            if (string.IsNullOrEmpty(register.Email) &&
                !string.IsNullOrEmpty(register.Username) &&
                CommonHelper.IsValidEmail(register.Username))
                register.Email = register.Username;

            if (string.IsNullOrEmpty(register.Username))
                register.Username = register.Email;

            var user = new User
            {
                Email = register.Email,
                UserName = register.Username,
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                var userToReturn = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userToReturn.Roles = roles;
                userToReturn.Token = _jwtGenerator.CreateToken(user, roles);
                var response = RestResponse.SuccessResponse;
                response.Data = userToReturn;
                return response;
            }
            string error = "";
            if (result.Errors != null)
            {
                foreach (var item in result.Errors)
                {
                    if (item.Code == "PasswordTooShort")
                        error += "Passwords must be at least 8 characters.\n";
                    if (item.Code == "PasswordRequiresDigit")
                        error += "Passwords must have at least one digit ('0'-'9').\n";
                    if (item.Code == "PasswordRequiresUpper")
                        error += "Passwords must have at least one uppercase ('A'-'Z').\n";
                }
            }
            if (string.IsNullOrEmpty(error))
            {
                error = "Problem creating user";
            }
            throw new Exception(error);
        }

        public async Task<RestResponse> Signin(Signin signin)
        {
            if (string.IsNullOrEmpty(signin.Email) && string.IsNullOrEmpty(signin.Username))
                throw new RestException(HttpStatusCode.BadRequest, "Please enter email/username");
            User user = null;

            if (!string.IsNullOrEmpty(signin.Email))
            {
                if (!CommonHelper.IsValidEmail(signin.Email))
                    throw new RestException(HttpStatusCode.BadRequest, "Email is not valid.");

                user = await _userManager.FindByEmailAsync(signin.Email);
            }
            else
            if (!string.IsNullOrEmpty(signin.Username))
            {
                user = await _userManager.FindByNameAsync(signin.Username);
            }

            if (user == null && !string.IsNullOrEmpty(signin.Username) && CommonHelper.IsValidEmail(signin.Username))
            {
                user = await _userManager.FindByEmailAsync(signin.Username);
            }

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, signin.Password, false);

            if (result.Succeeded)
            {
                var userToReturn = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userToReturn.Roles = roles;
                userToReturn.Token = _jwtGenerator.CreateToken(user, roles);
                var response = RestResponse.SuccessResponse;
                response.Data = userToReturn;
                return response;
            }

            throw new RestException(HttpStatusCode.Unauthorized, "The password is incurrect");
        }
    }
}
