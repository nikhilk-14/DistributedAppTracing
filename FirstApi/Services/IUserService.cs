using FirstApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstApi.Services
{
    public interface IUserService
    {
        User GetUser(int id);
        List<User> GetUsers();
        int DeleteUser(int id);
        int CreateUser(User user);
        int UpdateUser(int id, User user);
    }

    public interface IUserDataService
    {
        Task<User> GetUser(int id);
        Task<List<User>> GetUsers();
        Task<int> DeleteUser(int id);
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(int id, User user);
    }
}
