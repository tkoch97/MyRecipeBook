using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Attributes;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers
{
    [AuthenticatedUser]
    public class RecipeController : MyRecipeBookController
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseRegisteredRecipeJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register
            (
                [FromServices] IRegisterRecipeUseCase useCase, 
                [FromBody] RequestRecipeJson request
            )
        {
            var useCaseResponse = await useCase.Execute(request);

            return Created(string.Empty, useCaseResponse);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(ResponseRecipesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Filter
            (
                [FromServices] IFilterRecipeUseCase useCase,
                [FromBody] RequestFilterRecipeJson request
            )
        {
            var useCaseResponse = await useCase.Execute(request);

            if (useCaseResponse.Recipes.Any())
            {
                return Ok(useCaseResponse);
            }

            return NoContent();
        }
    }
}
