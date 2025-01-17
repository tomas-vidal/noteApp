using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using noteApp.backend.Models;
using System.Configuration;

namespace noteApp.backend.Helpers
{
    public class JwtServices
    {
        private readonly string privateKey;
        public JwtServices(IConfiguration configuration)
        {
            privateKey = configuration["Jwt:Key"];
        }
        public string Create(User user)
        {
            var handler = new JwtSecurityTokenHandler();

            var privateKeyEncoded = Encoding.UTF8.GetBytes(privateKey);

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(privateKeyEncoded),
                SecurityAlgorithms.HmacSha256);

            ClaimsIdentity ci = new ClaimsIdentity();
            ci.AddClaim(new Claim("Id", user.Id.ToString()));
            ci.AddClaim(new Claim(ClaimTypes.Name, user.Username));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(1),
                Subject = ci
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        public virtual ClaimsPrincipal Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(privateKey);

            var cp = tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return cp;
        }

    }
}
