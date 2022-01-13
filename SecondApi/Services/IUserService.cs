using SecondApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecondApi.Services
{
    public interface IUserService
    {
        User GetUser(int id);
        List<User> GetUsers();
        int DeleteUser(int id);
        int CreateUser(User user);
        int UpdateUser(int id, User user);
    }
}
