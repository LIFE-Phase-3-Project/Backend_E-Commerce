using AutoMapper;
using Domain.DTOs.User;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var userToRegister = _mapper.Map<User>(u);
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                throw new InvalidOperationException("User with this Email already exists");
            }

            var hashedPass = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password);

            userToRegister.Password = hashedPass;
            userToRegister.RoleId = 3;


            _unitOfWork.Repository<User>().Create(userToRegister);
            await _unitOfWork.CompleteAsync();
        }

        public Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        //public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        //{

        //    var user = _appDBContext.Users.FirstOrDefault(u => u.Id == userId);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
        //    {
        //        Console.WriteLine("Old password does not match.");
        //        return false;
        //    }

        //    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

        //    await _appDBContext.SaveChangesAsync();

        //    return true;
        //}

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

        public async Task<User> GetUserById(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Id == id).FirstOrDefaultAsync();
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

        public Task<User> UpdateUser(User objUser)
        {
            throw new NotImplementedException();
        }

        //public async Task<User> UpdateUser(User objUser)
        //{
        //    // Kontrollo për ekzistencën e përdoruesit
        //    var existingUser = await _unitOfWork.Repository<User>().GetById(objUser.Id);
        //    if (existingUser == null)
        //    {
        //        throw new InvalidOperationException("User not found");
        //    }

        //    // Kontrollo për ndryshim në fjalëkalim
        //    if (objUser.Password != existingUser.Password)
        //    {
        //        // Enkripto fjalëkalimin e ri
        //        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(objUser.Password);
        //        objUser.Password = hashedPassword;
        //    }

        //    // Bëj update të përdoruesit në bazën e të dhënave
        //    _unitOfWork.Repository<User>().Update(objUser);
        //    await _unitOfWork.CompleteAsync();

        //    return objUser;
        //}
    }
}
