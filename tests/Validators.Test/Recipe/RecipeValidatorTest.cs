using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.Recipe
{
    public class RecipeValidatorTest
    {
        [Fact]
        public void Validate_Should_ReturnsValidResult_When_AllFields_AreCorrect() 
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_CookingTime_IsInvalid()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();

            request.CookingTime = (MyRecipeBook.Communication.Enums.CookingTime?)10;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.COOKING_TIME_NOT_SUPPORTED))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_Difficulty_IsInvalid()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();

            request.Difficulty = (MyRecipeBook.Communication.Enums.Difficulty?)20;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DIFFICULTY_LEVEL_NOT_SUPPORTED))
                );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_Should_ReturnsAnError_When_Title_IsInvalid(string title)
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();

            request.Title = title;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.RECIPE_TITLE_EMPTY))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsValidResult_When_CookingTime_IsNull()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = null;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsValidResult_When_Difficulty_IsNull()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = null;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsValidResult_When_DishType_IsEmpty()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.DishTypes.Clear();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_Ingredients_IsEmpty()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients.Clear();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.RECIPE_MUST_HAVE_AT_LEAST_ONE_INGREDIENT))
                );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_Should_ReturnsAnError_When_A_Ingredient_Value_IsEmpty(string ingredient)
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients[0] = ingredient;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INGREDIENT_EMPTY))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_Instructions_IsEmpty()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.Clear();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.RECIPE_MUST_HAVE_AT_LEAST_ONE_INSTRUCTION))
                );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_Should_ReturnsAnError_When_A_Instruction_IsEmpty(string instructionText)
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions[0].Text = instructionText;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INSTRUCTION_TEXT_EMPTY))
                );
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_Should_ReturnsAnError_When_Instructions_Step_Is_Zero_Null_Or_Negative(int step)
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions[1].Step = step;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INSTRUCTION_STEP_MUST_BE_GREATER_THAN_ZERO))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_Instruction_Text_Exceeds_2000Characters()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions[0].Text = RequestStringGenerator.Paragraphs(minCharacters: 2001);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INSTRUCTION_TEXT_EXCEEDS_LIMIT_CHARACTERS))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_Instructions_Step_NotBe_Unique()
        {
            var validator = new RecipeValidator();

            //var instructions = new List<RequestInstructionJson>
            //{
            //    new RequestInstructionJson { Step = 1, Text = "Instruction 1" },
            //    new RequestInstructionJson { Step = 1, Text = "Instruction 2" },
            //    new RequestInstructionJson { Step = 3, Text = "Instruction 3" }
            //};

            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Step = request.Instructions.Last().Step; //Forma mais simples de deixar dois steps iguais

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INSTRUCTION_STEPS_MUST_BE_UNIQUE))
                );
        }

        [Fact]
        public void Validate_Should_ReturnsAnError_When_DishTypes_IsInvalid()
        {
            var validator = new RecipeValidator();

            var request = RequestRecipeJsonBuilder.Build();

            request.DishTypes.Add((DishType)100);

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions
                (
                    e => e.ShouldHaveSingleItem(),
                    e => e.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DISH_TYPE_NOT_SUPPORTED))
                );
        }
    }
}
