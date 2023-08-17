using JWTAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuth.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController: ControllerBase
    {
        public IConfiguration _configuration;
        private readonly DatabaseContext _databaseContext;
        public TokenController(IConfiguration configuration, DatabaseContext databaseContext)
        {
            _configuration = configuration;
            _databaseContext = databaseContext;
        }

        private async Task<UserInfo> GetUser(string email, string password)
        {
            return await _databaseContext.UserInfo.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserInfo user)
        {
            if (user != null && user.Email != null && user.Password != null)
            {
                var account = await GetUser(user.Email, user.Password);
                if(account != null)
                {
                    var claims = new[]
                    {
                        //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        //new Claim("UserId", user.UserId.ToString()),
                        //new Claim("DisplayName", user.DisplayName),
                        new Claim("Password", user.Password),
                        new Claim("Email", user.Email)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims, expires: DateTime.UtcNow.AddMinutes(10), signingCredentials:signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }

            else
            {
                return BadRequest();
            }
        }
    }
}
