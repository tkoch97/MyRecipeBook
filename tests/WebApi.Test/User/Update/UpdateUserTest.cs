using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.User.Update
{
    public class UpdateUserTest : MyRecipeBookClassFixture
    {
        private readonly string route = "user/update-profile";
        private readonly Guid _userIdentifier;

        public UpdateUserTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Put_Should_ReturnsNoContentStatusCode_When_RequestIsValid()
        {
            var request = RequestUpdateUserJsonBuilder.Build();

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPut(route, request, token);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Put_Should_ReturnsBadRequestStatusCodeAndAnMsgError_When_NameIsEmpty(string culture)
        {
            var request = RequestUpdateUserJsonBuilder.Build();

            request.Name = string.Empty;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
            var response = await DoPut(route, request, token, culture);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());
            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }
    }
}
