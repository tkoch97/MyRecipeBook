using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.Recipe.Register
{
    public class RegisterRecipeTest : MyRecipeBookClassFixture
    {
        private readonly string route = "recipe/register";
        private readonly Guid _userIdentifier;

        public RegisterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Post_Should_ReturnsCreatedStatusCode_When_RequestIsValid()
        {
            var request = RequestRecipeJsonBuilder.Build();

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPost(route: route, request: request, token: token);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Post_Should_ReturnsBadRequestStatusCode_When_TitleIsEmpty(string culture)
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPost(route: route, request: request, token: token, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }
    }
}
