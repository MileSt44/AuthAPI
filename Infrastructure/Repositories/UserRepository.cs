using AuthAPI.Infrastructure.Context;
using AuthAPI.Infrastructure.Interfaces;
using AuthAPI.Model.Model;

namespace AuthAPI.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(UsersContext context) : base(context) { }

        public User Login(string username, string password)
        {
            return context.Users.SingleOrDefault(user => (user.Username.ToLower() == username.ToLower() || user.Email.ToLower() == username.ToLower()) && user.Password == password);
        }
    }
}
