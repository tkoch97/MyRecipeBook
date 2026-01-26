using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Services.LoggedUser
{
    public interface ILoggedUser
    {
        public Task<User> User(); //Task q vai devolver a entidade User do domínio, o usuário logado no sistema.
    }
}
