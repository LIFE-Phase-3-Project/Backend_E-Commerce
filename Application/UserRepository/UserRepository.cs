using Domain.Entities;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task AddUser(User u)
        {
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                throw new InvalidOperationException("User with this Email already exists");
            }
            
            _unitOfWork.Repository<User>().Create(u);
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

        public async Task<User> UpdateUser(User objUser)
        {
            _unitOfWork.Repository<User>().Update(objUser);
            await _unitOfWork.CompleteAsync();
    
            return objUser;
        }
    }
}
