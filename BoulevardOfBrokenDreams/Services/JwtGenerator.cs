using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoulevardOfBrokenDreams.Services
{
    public class JwtGenerator
    {
        private readonly IConfiguration _config;

        public JwtGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(string username, string role, int id)
        {
            List<Claim> claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier,id.ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("JwtSettings:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _config.GetSection("JwtSettings:Issuer").Value,
                audience: _config.GetSection("JwtSettings:Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        public string decodeJwtId(string jwt)
        {
            jwt = jwt.Replace("Bearer ", "");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return id!;
        }
    }
}
