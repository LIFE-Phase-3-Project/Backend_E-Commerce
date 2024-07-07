using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserRepository
{
    public interface IUserRepository
    {
        Task AddUser(User u);
        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUserById(int id);

        Task<IEnumerable<User>> GetUsersByRoleId(int roliId);
        Task<User> UpdateUser(User objUser);
        bool DeleteUser(int ID);

        string GetUserRole(int roleId);
        //Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
