using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.Register
{
    // Todo teste precisa ser independente: ele cria o que precisa, executa/chama o que precisa e valida o que precisa

    public class RegisterUserValidatorTest
    {
        [Fact] // Indica que é um método de teste
        public void Validate_Should_ReturnsValidResult_When_AllFieldsAreCorrect() 
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_NameIsEmpty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.NAME_EMPTY))
            );
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_EmailIsEmpty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.EMAIL_EMPTY))
            );
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_EmailIsInvalid()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "nome.com";

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.EMAIL_VALID))
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Validate_Should_ReturnsError_When_PasswordIsInvalid(int passwordLength)
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build(passwordLength);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PASSWORD_LENGTH))
            );
        }
    }
}
