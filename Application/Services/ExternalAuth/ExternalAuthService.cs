using System.Security.Claims;
using Application;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Application.Services.ExternalAuth;
using Application.TokenService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Life_Ecommerce.Services
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExternalAuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> GoogleActionAsync(HttpContext context, string callbackUrl, string redirectUrl)
        {
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                return new RedirectResult(callbackUrl);
            }
            else
            {
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return new ChallengeResult("Google", properties);
            }
        }

        public async Task<IActionResult> GoogleLoginCallbackAsync(HttpContext context)
        {
            var authenticateResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                return new RedirectResult("/auth/login");
            }

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return new BadRequestResult();
            }

            var user = await _unitOfWork.Repository<User>()
                .GetByCondition(x => x.Email == email)
                .Include(x => x.UserRole)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                var address = authenticateResult.Principal.FindFirstValue(ClaimTypes.Country) ?? "Default Address";
                var phoneNumber = authenticateResult.Principal.FindFirstValue(ClaimTypes.MobilePhone) ?? "123";
                var profilePictureUrl = authenticateResult.Principal.FindFirstValue("urn:google:picture");
                var placeholderPassword = GenerateSecurePassword();
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(placeholderPassword);

                user = new User
                {
                    Email = email,
                    FirstName = authenticateResult.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Surname),
                    Password = hashedPassword,
                    RoleId = 1,
                    Address = address,
                    PhoneNumber = phoneNumber,
                };
                _unitOfWork.Repository<User>().Create(user);
                await _unitOfWork.CompleteAsync();

                user = await _unitOfWork.Repository<User>()
                    .GetByCondition(x => x.Email == email)
                    .Include(x => x.UserRole)
                    .FirstOrDefaultAsync();
            }

            var roleName = user.UserRole?.RoleName;
            if (roleName == null)
            {
                return new BadRequestResult();
            }

            var token = TokenService.GenerateToken(user.Id, roleName, email);
            var frontendUrl = $"http://localhost:3000/auth/callback?token={token}";

            return new RedirectResult(frontendUrl);
        }

        public async Task<IActionResult> GetTokenAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return new UnauthorizedResult();
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return new BadRequestResult();
            }

            var user = await _unitOfWork.Repository<User>()
                .GetByCondition(x => x.Email == email)
                .Include(x => x.UserRole)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new UnauthorizedResult();
            }

            var roleName = user.UserRole?.RoleName;
            if (roleName == null)
            {
                return new BadRequestResult();
            }

            var token = TokenService.GenerateToken(user.Id, roleName, email);
            return new OkObjectResult(new { token });
        }

        public async Task<IActionResult> WhoAmIAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return new UnauthorizedResult();
            }

            var claims = jsonToken.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            return new OkObjectResult(claims);
        }
        
        
        private string GenerateSecurePassword(int length = 42)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-=";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
