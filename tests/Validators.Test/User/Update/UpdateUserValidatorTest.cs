using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.Update
{
    public class UpdateUserValidatorTest
    {
        [Fact]
        public void Validade_Should_ReturnsValidResult_When_AllFieldsAreCorrect()
        {
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_NameIsEmpty()
        {
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;
            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.NAME_EMPTY))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_EmailIsEmpty()
        {
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = string.Empty;
            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.EMAIL_EMPTY))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsError_When_EmailIsInvalid()
        {
            var validator = new UpdateUserValidator();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = "name.com";
            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.EMAIL_VALID))
                );
        }
    }
}
