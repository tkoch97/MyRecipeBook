using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.User.Register
{
    public class RegisterUserTest : MyRecipeBookClassFixture
    {
        private readonly string userRoute = "user";

        public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory){}
        
        [Fact]
        public async Task Post_Should_ReturnsCreatedStatusCodeAndResponseContent_When_RequestIsValid()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            var response = await DoPost($"{userRoute}/register", request);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
                name => name.ShouldNotBeNullOrEmpty(),
                name => name.ShouldBe(request.Name)
                );
            responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();

        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Post_Should_ReturnsBadRequestStatusCodeAndAnMsgError_When_NameIsEmpty(string culture)
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var response = await DoPost(route: $"{userRoute}/register", request: request, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);


            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.First().ShouldBe(responseErrorMsgExpected)
                );
        }
    }
}
