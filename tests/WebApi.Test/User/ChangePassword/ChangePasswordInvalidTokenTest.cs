using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string route = "user/change-password";

        public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Should_ReturnsUnauthorized_When_TokenIsIvalid(string culture)
        {
            var request = RequestChangePasswordJsonBuilder.Build();

            var response = await DoPut(route, request, token: "invalid_token", culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.
                GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

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
        public async Task Should_ReturnsUnauthorized_When_TokenIsEmpty(string culture)
        {
            var request = RequestChangePasswordJsonBuilder.Build();

            var response = await DoPut(route, request, token: "", culture);

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
        public async Task Should_ReturnsUnauthorized_When_UserNotFound(string culture)
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());
            var request = RequestChangePasswordJsonBuilder.Build();

            var response = await DoPut(route, request, token, culture);

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
