using AuthAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Infrastructure.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User Login(string username, string password);
    }
}
