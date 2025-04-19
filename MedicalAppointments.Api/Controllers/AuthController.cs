using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Shared.DTOs.Auth;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedicalAppointments.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserHelper _helper;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IConfiguration configuration,
                              ICurrentUserHelper helper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _helper = helper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _helper.GetCurrentUserAsync();
            var role = await _helper.GetUserRoleAsync();

            switch (role)
            {
                case "Patient":
                    user = user as Patient;
                    break;
                case "Doctor":
                    user = user as Doctor;
                    break;
                case "Admin":
                    user = user as Admin;
                    break;
                case "SuperAdmin":
                    break;
                default:
                    return BadRequest("Invalid user role.");
            }

            if (user is not ApplicationUser applicationUser)
                return BadRequest("Invalid user type.");

            UserViewModel userViewModel = new()
            {
                Id = applicationUser.Id,
                Title = applicationUser.Title!,
                FirstName = applicationUser.FirstName!,
                LastName = applicationUser.LastName!,
                Role = role
            };

            return Ok(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null)
                return BadRequest(new AuthResponseDto { Successful = false, Error = "User not found." });

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            if (!result.Succeeded) 
                return BadRequest(new AuthResponseDto { Successful = false, Error = "Invalid login." });

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto { Successful = true, Token = await GenerateJwtToken(user, login.RememberMe), Roles = roles });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user, bool rememberMe)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = rememberMe
                ? DateTime.UtcNow.AddDays(7)
                : DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"]!));

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new("fullname", user.FullName)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}