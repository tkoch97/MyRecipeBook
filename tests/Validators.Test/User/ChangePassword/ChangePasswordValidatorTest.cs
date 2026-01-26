using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.ChangePassword
{
    public class ChangePasswordValidatorTest
    {
        [Fact]
        public void Validate_Should_ReturnsValidResult_When_AllFieldsAreCorrect()
        {
            var validator = new ChangePasswordValidator();

            var request = RequestChangePasswordJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_NewPasswordIsSameAsCurrentPassword()
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();

            request.NewPassword = request.CurrentPassword;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PASSWORDS_MUST_BE_DIFFERENT))
            );
        }

        [Theory]
        [InlineData(5)]
        [InlineData(3)]
        [InlineData(1)]
        public void Validate_Should_ReturnsError_When_NewPasswordIsTooShort(int newPasswordLength)
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build(newPasswordLenght: newPasswordLength);
            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PASSWORD_LENGTH))
            );

        }

        [Fact]
        public void Validate_Should_ReturnsError_When_NewPasswordIsEmpty()
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();

            request.NewPassword = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PASSWORD_EMPTY))
            );
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_CurrentPasswordIsEmpty()
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();

            request.CurrentPassword = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                e => e.ShouldHaveSingleItem(),
                e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PASSWORD_EMPTY))
            );
        }

    }
}
