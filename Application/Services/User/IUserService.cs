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
        Task<UserWithRoleDto> GetUserById(int id);

        Task<IEnumerable<Domain.Entities.User>> GetUsersByRoleId(int roliId);
        Task<Domain.Entities.User> UpdateUser(Domain.Entities.User objUser);
        Task<bool> DeleteUser(int id);

        string GetUserRole(int roleId);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
