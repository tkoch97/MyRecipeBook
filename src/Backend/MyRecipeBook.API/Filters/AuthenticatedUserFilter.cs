using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters
{
    public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
    {
        private readonly IAccessTokenValidator _accessTokenValidator;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;

        public AuthenticatedUserFilter(IAccessTokenValidator accessTokenValidator, IUserReadOnlyRepository userReadOnlyRepository)
        {
            _accessTokenValidator = accessTokenValidator;
            _userReadOnlyRepository = userReadOnlyRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            try
            {

                var token = TokenOnRequest(context);

                var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

                var userExists = await _userReadOnlyRepository.ExistActiveUserWithUserIdentifier(userIdentifier);

                if (!userExists)
                {
                    throw new MyRecipeBookException(ResourceMessageException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
                }
            }
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("TokenIsExpired")
                {
                    TokenIsExpired = true
                });
            }
            catch (MyRecipeBookException ex)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
            }
            catch
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessageException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
            }
        }

        private static string TokenOnRequest(AuthorizationFilterContext context)
        {
            var authentication = context.HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authentication))
            {
                throw new MyRecipeBookException(ResourceMessageException.NO_TOKEN);
            }

            return authentication["Bearer".Length..].Trim();
        }
    }
}
