using Dotnet7API.Modal;
using Dotnet7API.Repos;
using Dotnet7API.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dotnet7API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearndataContextb _context;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshHandler _refreshHandler;

        public AuthorizeController(LearndataContextb context, IOptions<JwtSettings> options, IRefreshHandler refreshHandler)
        {
            _context = context;
            _jwtSettings = options.Value;
            _refreshHandler = refreshHandler;
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCredentials userCredentials)
        {
            var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Code == userCredentials.Username && item.Password == userCredentials.Password);
            if (user != null)
            {
                //generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
                var tokenDesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Code),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms .HmacSha256)
                };
                var token = tokenHandler.CreateToken(tokenDesc);
                var finalToken = tokenHandler.WriteToken(token);
                
                return Ok(new TokenResponse() { Token=finalToken, RefreshToken= await _refreshHandler.GenerateToken(userCredentials.Username)});
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] TokenResponse token)
        {
            var refreshToken = await _context.TblRefreshTokens.FirstOrDefaultAsync(item => item.RefreshToken == token.RefreshToken);
            
            if (refreshToken != null)
            {
                //generate refreshtoken
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                }, out securityToken);

                var newToken = securityToken as JwtSecurityToken;
                if (newToken != null && newToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string username = principal.Identity.Name;
                    var existdata = _context.TblRefreshTokens.FirstOrDefaultAsync(item => item.UserId == username && item.RefreshToken == token.RefreshToken);
                    if (existdata != null)
                    {
                        var newwToken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(45),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey)),
                            SecurityAlgorithms.HmacSha256)                       
                            );

                        var finalToken = tokenHandler.WriteToken(newwToken);
                        return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await _refreshHandler.GenerateToken(username)});
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
