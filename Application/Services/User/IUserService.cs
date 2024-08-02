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
        Task<IEnumerable<Domain.Entities.User>> GetUsers();
        Task<Domain.Entities.User> GetUserById(int id);
        Task<IEnumerable<Domain.Entities.User>> GetUsersByRoleId(int roliId);
        Task<Domain.Entities.User> UpdateUser(string token, Domain.Entities.User objUser);
        Task<bool> DeleteUser(string token);
        string GetUserRole(int roleId);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
        Domain.Entities.User AuthenticateUser(string email, string password);
        string GenerateToken(int userId, string roleName, string email);
    }
}