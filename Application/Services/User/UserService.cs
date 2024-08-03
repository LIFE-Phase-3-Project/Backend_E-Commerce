using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Domain.DTOs.Pagination;
using Domain.DTOs.User;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Presistence;
using Stripe.Issuing;

namespace Application.Services.UserRepository
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        
        public async Task AddUser(RegisterUserDto u)
        {
            var userToRegister = _mapper.Map<Domain.Entities.User>(u);
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                throw new InvalidOperationException("User with this Email already exists");
            }

            var hashedPass = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password);

            userToRegister.Password = hashedPass;

            _unitOfWork.Repository<Domain.Entities.User>().Create(userToRegister);
            await _unitOfWork.CompleteAsync();
        }
        
        
        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                Console.WriteLine("Old password does not match.");
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _unitOfWork.Repository<Domain.Entities.User>().Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }


        public async Task<bool> DeleteUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId =int.Parse(jwtToken.Claims.First(claim => claim.Type == "nameid").Value);

            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                _unitOfWork.Repository<Domain.Entities.User>().Delete(user);
                var deleted = _unitOfWork.Complete();
                return deleted;
            }
            return false;
        }


        public async Task<Domain.Entities.User> GetUserById(int id)
        {
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public string GetUserRole(int roleId)
        {
            var roleName = _unitOfWork.Repository<Role>().GetByCondition(x => x.Id == roleId).Select(r => r.RoleName).FirstOrDefault();

            if (roleName == null)
            {
                throw new KeyNotFoundException("Role not found");
            }

            return roleName;
        }

        public async Task<PaginatedInfo<Domain.Entities.User>> GetUsers(int page, int pageSize)
        {
            var query = _unitOfWork.Repository<Domain.Entities.User>().GetAll();
            var totalCount = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(); ;
            var paginatedInfo = new PaginatedInfo<Domain.Entities.User>
            {
               Items = products,
               Page = page,
               PageSize = pageSize,
               TotalCount = totalCount
            };
            return paginatedInfo;
        }

        public async Task<IEnumerable<Domain.Entities.User>> GetUsersByRoleId(int roleId)
        {
            return await _unitOfWork.Repository<Domain.Entities.User>()
                .GetByCondition(user => user.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<Domain.Entities.User> UpdateUser(string token, Domain.Entities.User objUser)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId =int.Parse(jwtToken.Claims.First(claim => claim.Type == "nameid").Value);

            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException("You don't have access to delete this account");
            }
            
            if (objUser.Password != user.Password)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(objUser.Password);
                user.Password = hashedPassword;
            }

            _mapper.Map(objUser, user);

            _unitOfWork.Repository<Domain.Entities.User>().Update(user);
            await _unitOfWork.CompleteAsync();

            return user;
        }
        
        public Domain.Entities.User AuthenticateUser(string email, string password)
        {
            var user = _unitOfWork.Repository<Domain.Entities.User>().GetByCondition(x => x.Email == email).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            return user;
        }

        public string GenerateToken(int userId, string roleName, string email)
        {
            return Life_Ecommerce.TokenService.TokenService.GenerateToken(userId, roleName, email);
            // return TokenService.GenerateToken(userId, roleName, email);
        }

    }
}
