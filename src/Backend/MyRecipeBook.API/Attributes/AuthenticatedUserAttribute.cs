using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;

namespace MyRecipeBook.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthenticatedUserAttribute : TypeFilterAttribute
    {
        public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
        {

        }
    }
}
