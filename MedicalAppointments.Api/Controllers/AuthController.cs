using MedicalAppointments.Shared.DTOs.Auth;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Http;
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

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

            return Ok(new AuthResponseDto { Successful = true, Token = GenerateJwtToken(user, login.RememberMe) });
        }

        private string GenerateJwtToken(ApplicationUser user, bool rememberMe)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = rememberMe
                ? DateTime.UtcNow.AddDays(7)
                : DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims:
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!)
                ],
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}