using Domain.DTOs.Pagination;
using Domain.DTOs.User;

namespace Application.Services.UserRepository
{
    public interface IUserService
    {
        Task AddUser(RegisterUserDto u);
        Task<PaginatedInfo<UserDto>> GetUsers(int page, int pageSize);
        Task<UserDto> GetUserById(string id);
        Task<IEnumerable<UserDto>> GetUsersByRole(string role);
        Task<UserDto> UpdateUser(string token, UpdateUserDto objUser);
        Task<UserDto> ChangeRole(string userId, string newRole);

        Task<bool> DeleteUser(string token);
        Task<bool> ChangePassword(string token, string oldPassword, string newPassword);
        Domain.Entities.User AuthenticateUser(string email, string password);
        string GenerateToken(string userId, string roleName, string email);
    }
}