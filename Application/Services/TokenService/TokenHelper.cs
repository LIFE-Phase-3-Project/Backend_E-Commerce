using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Application.Services.TokenService
{
    public class TokenHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userId = jwtToken.Claims.First(claim => claim.Type == "sub").Value;
                return userId;
            }

            return null;
        }

        public string GetUserRole()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    var role = jwtToken.Claims.First(claim => claim.Type == "https://ecommerce-life-2.com/role").Value;
                    return role;
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    // _logger.LogError(ex, "Error reading user role from token");
                    return null;
                }
            }
            return null;
        }
    }
}