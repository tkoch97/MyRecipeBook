using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordTest : MyRecipeBookClassFixture
    {
        private readonly string route = "user/change-password";
        private readonly Guid _userIdentifier;
        private readonly string _password;
        private readonly string _email;
        private readonly RequestChangePasswordJson _request;
        private readonly string _token;

        public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _password = factory.GetPassword();
            _email = factory.GetEmail();
            _request = RequestChangePasswordJsonBuilder.Build();
            _token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
        }

        [Fact]
        public async Task Put_Should_ReturnsNoContentStatusCode_When_RequestIsValid()
        {
            _request.CurrentPassword = _password;

            var response = await DoPut(route: route, request:_request, token: _token);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var loginRequest = new RequestLoginUserJson
            {
                Email = _email,
                Password = _password
            };

            var loginResponse = await DoPost(route: "login", request: loginRequest);

            loginResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            loginRequest.Password = _request.NewPassword;

            loginResponse = await DoPost(route: "login", request: loginRequest);
            loginResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Put_Should_ReturnsBadRequestStatusCode_And_ErrorMsg_When_CurrentPasswordIsInvalid(string culture)
        {
            var response = await DoPut(route: route, request: _request, token: _token, culture: culture);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager
                .GetString("CURRENT_PASSWORD_DIFFERENT_REGISTERED_PASSWORD", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("pt-BR")]
        [InlineData("es-ES")]
        public async Task Put_Should_ReturnsBadRequestStatusCode_And_ErrorMsg_When_NewPassword_IsEmpty(string culture)
        {
            _request.NewPassword = string.Empty;
            _request.CurrentPassword = _password;

            var response = await DoPut(route: route, request: _request, token: _token, culture:culture);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

            var responseErrorMsgExpected = ResourceMessageException.ResourceManager
                .GetString("PASSWORD_EMPTY", new CultureInfo(culture));

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(responseErrorMsgExpected)
                );
        }
    }
}
