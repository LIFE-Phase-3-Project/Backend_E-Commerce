using AutoMapper;
using Domain.DTOs.User;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Presistence;

namespace Application.Services.UserRepository
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly APIDbContext _appDBContext;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, APIDbContext context)
        {
            _unitOfWork = unitOfWork;
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;

        }



        public async Task AddUser(RegisterUserDto u)
        {
            var userToRegister = _mapper.Map<User>(u);
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                throw new InvalidOperationException("User with this Email already exists");
            }

            var hashedPass = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password);

            userToRegister.Password = hashedPass;



            _unitOfWork.Repository<User>().Create(userToRegister);
            await _unitOfWork.CompleteAsync();
        }



        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _unitOfWork.Repository<User>().GetById(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                Console.WriteLine("Old password does not match.");
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }


        public async Task<bool> DeleteUser(int id)
        {

            var user = await _unitOfWork.Repository<User>().GetById(x => x.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {

                _unitOfWork.Repository<User>().Delete(user);
                var deleted = _unitOfWork.Complete();
                return deleted;
            }


            return false;
        }


        public async Task<UserWithRoleDto> GetUserById(int id)
        {
            var userWithRole = await _unitOfWork.Repository<User>()
                .GetByCondition(user => user.Id == id)
                .Join(_unitOfWork.Repository<Role>().GetAll(),
                      user => user.RoleId,
                      role => role.Id,
                      (user, role) => new UserWithRoleDto
                      {
                          Id = user.Id,
                          FirstName = user.FirstName,
                          LastName = user.LastName,
                          Email = user.Email,
                          PhoneNumber = user.PhoneNumber,
                          Address = user.Address,
                          Password = user.Password,
                          RoleName = role.RoleName
                      })
                .FirstOrDefaultAsync();

            return userWithRole;
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _unitOfWork.Repository<User>().GetAll().ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleId(int roleId)
        {
            return await _unitOfWork.Repository<User>()
                .GetByCondition(user => user.RoleId == roleId)
                .ToListAsync();
        }



        public async Task<User> UpdateUser(User objUser)
        {
            var existingUser = await _unitOfWork.Repository<User>().GetById(x => x.Id == objUser.Id).FirstOrDefaultAsync();
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

            _unitOfWork.Repository<User>().Update(existingUser);
            await _unitOfWork.CompleteAsync();

            return existingUser;
        }

    }
}
