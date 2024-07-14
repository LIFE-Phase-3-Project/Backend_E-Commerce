using Domain.DTOs.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserRepository
{
    public interface IUserService
    {
        Task AddUser(RegisterUserDto u);
        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUserById(int id);

        Task<IEnumerable<User>> GetUsersByRoleId(int roliId);
        Task<User> UpdateUser(User objUser);
        Task<bool> DeleteUser(int id);

        string GetUserRole(int roleId);
        //Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
