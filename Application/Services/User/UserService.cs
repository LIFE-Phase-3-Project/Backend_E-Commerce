using System.IdentityModel.Tokens.Jwt;
using Application.Services.Email;
using AutoMapper;
using Domain.DTOs.Pagination;
using Domain.DTOs.User;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Presistence;
using Stripe.Issuing;

namespace Application.Services.UserRepository
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
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
            
            var subject = "Welcome to Life Ecommerce!";
            var message = $@"
            Hi {user.FirstName},

            Welcome to the Life Ecommerce family!

            We are thrilled to have you with us. At Life Ecommerce, we strive to provide you with the best shopping experience possible. If you have any questions, need assistance, or just want to say hello, don't hesitate to reach out to our support team.

            Happy shopping!

            Best regards,
            The Life Ecommerce Team

            -- 
            Life Ecommerce
            Your go-to place for all your needs";

            await _emailService.SendEmailAsync(user.Email, subject, message);
        }
        
        
        public async Task<bool> ChangePassword(string token, string oldPassword, string newPassword)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            if (jwtToken == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
            if (userIdClaim == null)
            {
                throw new SecurityTokenException("UserId not found in token");
            }

            var userId = userIdClaim.Value;
            
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

            var userId = jwtToken.Claims.First(claim => claim.Type == "sub").Value;

            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                _unitOfWork.Repository<Domain.Entities.User>().Delete(user);
                var deleted = _unitOfWork.Complete();
                return deleted;
            }
            return false;
        }


        public async Task<UserDto> GetUserById(string id)
        {
            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == id).FirstOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(user);
            
            return userDto;
        }

        public async Task<PaginatedInfo<UserDto>> GetUsers(int page, int pageSize)
        {
            var query = _unitOfWork.Repository<Domain.Entities.User>().GetAll();
            var totalCount = await query.CountAsync();
            
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users).ToList();

            var paginatedInfo = new PaginatedInfo<UserDto>
            {
               Items = userDtos,
               Page = page,
               PageSize = pageSize,
               TotalCount = totalCount
            };
            return paginatedInfo;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRole(string role)
        {
            var roleName = "https://ecommerce-life-2.com/"+role;
            
            var users = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetByCondition(user => user.Role == roleName)
                .ToListAsync();
            
            var userResponse = _mapper.Map<IEnumerable<UserDto>>(users);
            return userResponse;
        }

        public async Task<UserDto> UpdateUser(string token, UpdateUserDto objUser)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.First(claim => claim.Type == "sub").Value;

            var user = await _unitOfWork.Repository<Domain.Entities.User>().GetById(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException("You don't have access to edit this account");
            }

            _mapper.Map(objUser, user);

            _unitOfWork.Repository<Domain.Entities.User>().Update(user);
            await _unitOfWork.CompleteAsync();

            var userDto = _mapper.Map<UserDto>(user);
            
            return userDto;
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

        public string GenerateToken(string userId, string roleName, string email)
        {
            return Life_Ecommerce.TokenService.TokenService.GenerateToken(userId, roleName, email);
            // return TokenService.GenerateToken(userId, roleName, email);
        }

    }
}
