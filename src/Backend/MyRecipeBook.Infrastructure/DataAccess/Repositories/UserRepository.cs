using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
    {
        private readonly MyRecipeBookDbContext _dbcontext;

        public UserRepository(MyRecipeBookDbContext dbcontext) => _dbcontext = dbcontext;

        // Write Only
        public async Task Add(User user) => await _dbcontext.Users.AddAsync(user);

        // Read Only
        public async Task<bool> ExistActiveUserWithEmail(string email)
        {
            return await _dbcontext.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);
        }

        public async Task<bool> ExistActiveUserWithUserIdentifier(Guid userIdentifier) => 
            await _dbcontext.Users.AnyAsync(user => user.UserIdentifier.Equals(userIdentifier) && user.Active);

        public async Task<User?> GetByEmailAndPassword(string email, string password)
        {
            return await _dbcontext.Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email.Equals(email) && user.Password.Equals(password) && user.Active);
        }

        // Update Only
        public async Task<User> GetById(long id)
        {
            return await _dbcontext.Users.
                FirstAsync(user => user.Id == id); //mais recomendado usar "==" do que ".Equals" para tipos primitivos
        }

        public void Update(User user) => _dbcontext.Users.Update(user);
    }
}
