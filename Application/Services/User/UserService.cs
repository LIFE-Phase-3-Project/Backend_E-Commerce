using Application.Services.Discount;
using Application.Services.User;
using AutoMapper;
using Domain.DTOs.Discount;
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
        private readonly IUserContext _userContext;
        private readonly IDiscountService _discountService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, APIDbContext context, IUserContext userContext, IDiscountService discountService)
        {
            _unitOfWork = unitOfWork;
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _discountService = discountService;
        }



        public async Task AddUser(RegisterUserDto u)
        {
            var userToRegister = _mapper.Map<Domain.Entities.User>(u);
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();_unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                throw new InvalidOperationException("User with this Email already exists");
            }

            var hashedPass = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password);

            userToRegister.Password = hashedPass;

             _unitOfWork.Repository<Domain.Entities.User>().Create(userToRegister);
            await _unitOfWork.CompleteAsync();
            var newUser = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Email == u.Email).FirstOrDefaultAsync();
            //  create a discount entity for the new user
            var discount = new CreateDiscountDto
            {
                UserId = newUser.Id,
                Code = "New User Discount",
                Percentage = 10,
                ExpiryDate = DateTime.Now.AddDays(7),
            };
            await _discountService.CreateDiscount(discount);
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
            var currentUserId = _userContext.GetCurrentUserId();
            if (currentUserId != id)
            {
                throw new UnauthorizedAccessException("You can only delete your own account.");
            }

            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            _unitOfWork.Repository<Domain.Entities.User>().Delete(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<UserWithRoleDto> GetUserById(int id)
        {
            var userWithRole = await _unitOfWork.Repository<Domain.Entities.User>()
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
            var currentUserId = _userContext.GetCurrentUserId();
            if (currentUserId != objUser.Id)
            {
                throw new UnauthorizedAccessException("You can only update your own account.");
            }

            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(u => u.Id == objUser.Id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.FirstName = objUser.FirstName;
            user.LastName = objUser.LastName;
            user.Email = objUser.Email;
            user.PhoneNumber = objUser.PhoneNumber;
            user.Address = objUser.Address;

            _unitOfWork.Repository<Domain.Entities.User>().Update(user);
            await _unitOfWork.CompleteAsync();

            return user;
        }
    }
}
