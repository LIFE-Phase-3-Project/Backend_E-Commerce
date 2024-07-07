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
        private readonly APIDbContext _appDBContext;
        public UserRepository(APIDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
            
        }


        public async Task AddUser(User u)
        {
            _appDBContext.Users.Add(u);
            await _appDBContext.SaveChangesAsync();


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

        public bool DeleteUser(int ID)
        {
            bool result = false;
            var user = _appDBContext.Users.Find(ID);
            if (user != null)
            {
                _appDBContext.Entry(user).State = EntityState.Deleted;
                _appDBContext.SaveChanges();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _appDBContext.Users.FindAsync(id);
        }

        public string GetUserRole(int roleId)
        {
            var roleName = _appDBContext.Roles.SingleOrDefault(r => r.Id == roleId).RoleName;
            return roleName;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _appDBContext.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleId(int roliId)
        {
            return await _appDBContext.Users.Where(user => user.RoleId == roliId).ToListAsync();
        }

        public async Task<User> UpdateUser(User objUser)
        {
            _appDBContext.Entry(objUser).State = EntityState.Modified;
            await _appDBContext.SaveChangesAsync();
            return objUser;
        }
    }
}
