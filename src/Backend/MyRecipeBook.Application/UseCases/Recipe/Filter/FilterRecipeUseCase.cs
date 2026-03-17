using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Services.LoggedUser;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter
{
    public class FilterRecipeUseCase : IFilterRecipeUseCase
    {
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;

        public FilterRecipeUseCase(IMapper mapper, ILoggedUser loggedUser)
        {
            _mapper = mapper;
            _loggedUser = loggedUser;
        }

        public Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
        {
            throw new NotImplementedException();
        }
    }
}
