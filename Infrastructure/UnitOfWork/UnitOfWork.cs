using AuthAPI.Infrastructure.Context;
using AuthAPI.Infrastructure.Interfaces;
using AuthAPI.Infrastructure.Repositories;

namespace AuthAPI.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private UsersContext context;
        public UnitOfWork(UsersContext context)
        {
            this.context = context;
            UserRepository = new UserRepository(this.context);
        }
        public IUserRepository UserRepository
        {
            get;
            private set;
        }

        public void Dispose()
        {
            context.Dispose();
        }
        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
