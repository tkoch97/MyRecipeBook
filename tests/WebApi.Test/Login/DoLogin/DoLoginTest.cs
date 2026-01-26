using CommonTestUtilities.Requests;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace WebApi.Test;

public class DoLoginTest : MyRecipeBookClassFixture
{   
    private readonly string loginRoute = "login";
    private readonly string _email;
    private readonly string _password;
    private readonly string _name;

    public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.GetEmail();
        _password = factory.GetPassword();
        _name = factory.GetName();
    }

    [Fact]
    public async Task Post_Should_ReturnsOkStatusCodeAndResponseContent_When_RequestIsValid()
    {
        var request = new RequestLoginUserJson{ Email = _email, Password = _password };

        var response = await DoPost(loginRoute, request);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
            name => name.ShouldNotBeNullOrEmpty(),
            name => name.ShouldBe(_name)
            );
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("pt-BR")]
    [InlineData("es-ES")]
    public async Task Post_Should_ReturnsUnauthorizedStatusCodeAndErrorMessage_When_UserIsNotFound(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();

        var response = await DoPost(loginRoute, request, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray().Select(e => e.GetString());

        var responseErrorMsgExpected = ResourceMessageException.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions
            (
                e => e.ShouldHaveSingleItem(),
                e => e.First().ShouldBe(responseErrorMsgExpected)
            );
    }
}
