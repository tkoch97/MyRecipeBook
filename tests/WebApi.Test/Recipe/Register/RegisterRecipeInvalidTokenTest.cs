using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.Recipe.Register
{
    public class RegisterRecipeInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string route = "recipe/register";

        public RegisterRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Should_Returns_Unauthorized_When_TokenIsInvalid(string culture)
        {
            var request = RequestRecipeJsonBuilder.Build();
            var response = await DoPost(route: route, request: request, token: "invalid_token", culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldNotBeNull(),
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Should_Returns_Unauthorized_When_TokenIsEmpty(string culture)
        {
            var request = RequestRecipeJsonBuilder.Build();
            var response = await DoPost(route: route, request: request, token: string.Empty, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("NO_TOKEN", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldNotBeNull(),
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Should_Returns_Unauthorized_When_UserNotFound(string culture)
        {
            var request = RequestRecipeJsonBuilder.Build();
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var response = await DoPost(route: route, request: request, token: token, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldNotBeNull(),
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }
    }
}

