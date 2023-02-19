namespace AuthAPI.Infrastructure.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        IUserRepository UserRepository { get; }
        int Save();
    }
}
