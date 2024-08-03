using Domain.DTOs.Pagination;
using Domain.DTOs.User;

namespace Application.Services.UserRepository
{
    public interface IUserService
    {
        Task AddUser(RegisterUserDto u);
        Task<PaginatedInfo<Domain.Entities.User>> GetUsers(int page, int pageSize);
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