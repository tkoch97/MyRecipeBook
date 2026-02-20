using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register
{
    public class RegisterRecipeUseCase : IRegisterRecipeUseCase
    {
        private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;

        public RegisterRecipeUseCase
            (
            IMapper mapper,
            IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
            IUnitOfWork unitOfWork,
            ILoggedUser loggedUser
            )
        {
            _mapper = mapper;
            _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request)
        {
            Validate(request);
            var loggedUser = await _loggedUser.User();
            var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
            recipe.UserId = loggedUser.Id;

            var recipeInstructions = request.Instructions.OrderBy(i => i.Step).ToList();
            for (var index = 0; index < recipeInstructions.Count; index++)
            {
                recipeInstructions[index].Step = index + 1;
            }

            recipe.Instructions = _mapper.Map<List<Domain.Entities.Instruction>>(recipeInstructions);

            await _recipeWriteOnlyRepository.Add(recipe);

            await _unitOfWork.Commit();

            return _mapper.Map<ResponseRegisteredRecipeJson>(recipe);
        }

        private static void Validate(RequestRecipeJson request)
        {
            var validate = new RecipeValidator();

            var result = validate.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();

                throw new ErrorOnValidationException(errorMessages);
            }

        }
    }
}
