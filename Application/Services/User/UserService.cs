using AutoMapper;
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


        public async Task<bool> DeleteUser(int id)
        {
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == id).FirstOrDefaultAsync();
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

        public async Task<IEnumerable<Domain.Entities.User>> GetUsers()
        {
            return await _unitOfWork.Repository<Domain.Entities.User>().GetAll().ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.User>> GetUsersByRoleId(int roleId)
        {
            return await _unitOfWork.Repository<Domain.Entities.User>()
                .GetByCondition(user => user.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<Domain.Entities.User> UpdateUser(Domain.Entities.User objUser)
        {
            var existingUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == objUser.Id).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found");
            }
            
            if (objUser.Password != existingUser.Password)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(objUser.Password);
                existingUser.Password = hashedPassword;
            }

            _mapper.Map(objUser, existingUser);

            _unitOfWork.Repository<Domain.Entities.User>().Update(existingUser);
            await _unitOfWork.CompleteAsync();

            return existingUser;
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
