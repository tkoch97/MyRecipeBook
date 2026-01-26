using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyRecipeBook.Infrastructure.Secutiry.Tokens.Access.Generator
{
    public class JwtTokenGenerator : JwtTokenHandler, IAccessTokenGenerator
    {
        private readonly uint _expirationTimeMinutes;
        private readonly string _signingKey;

        public JwtTokenGenerator(string signingKey, uint expirationTimeMinutes)
        {
            _signingKey = signingKey;
            _expirationTimeMinutes = expirationTimeMinutes;
        }

        public string Generate(Guid userIdentifier)
        {

            var Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, userIdentifier.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes),
                SigningCredentials = new SigningCredentials
                (
                    SecurityKey(_signingKey), 
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
