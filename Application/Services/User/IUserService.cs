using Domain.DTOs.Pagination;
using Domain.DTOs.User;

namespace Application.Services.UserRepository
{
    public interface IUserService
    {
        Task AddUser(RegisterUserDto u);
        Task<PaginatedInfo<Domain.Entities.User>> GetUsers(int page, int pageSize);
        Task<Domain.Entities.User> GetUserById(string id);
        Task<IEnumerable<Domain.Entities.User>> GetUsersByRole(string role);
        Task<Domain.Entities.User> UpdateUser(string token, Domain.Entities.User objUser);
        Task<bool> DeleteUser(string token);
        Task<bool> ChangePassword(string userId, string oldPassword, string newPassword);
        Domain.Entities.User AuthenticateUser(string email, string password);
        string GenerateToken(string userId, string roleName, string email);
    }
}