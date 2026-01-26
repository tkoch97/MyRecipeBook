using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyRecipeBook.Infrastructure.Secutiry.Tokens.Access
{
    public abstract class JwtTokenHandler
    {
        protected SymmetricSecurityKey SecurityKey(string signingKey)
        {
            var bytesArray = Encoding.UTF8.GetBytes(signingKey);

            return new SymmetricSecurityKey(bytesArray);
        }
    }
}
