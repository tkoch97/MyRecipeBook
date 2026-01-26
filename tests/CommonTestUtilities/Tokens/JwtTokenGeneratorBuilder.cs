using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Secutiry.Tokens.Access.Generator;

namespace CommonTestUtilities.Tokens
{
    public class JwtTokenGeneratorBuilder
    {
        public static IAccessTokenGenerator Build() => new JwtTokenGenerator(
            signingKey: "7f99cacec7413bd2b7295108fa5583b6f4c488382dad55607ccc43fa5222d3ed", 
            expirationTimeMinutes: 5);
    }
}
