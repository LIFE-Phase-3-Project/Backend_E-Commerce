using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Life_Ecommerce.TokenService
{
   
        public class TokenService
        {
            public static string GenerateToken(string id, string role, string email)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("OUR_SECRET_KEY_FROM_LIFE_FROM_GJIRAFA");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, id),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(ClaimTypes.Email, email)
                }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

            public static ClaimsPrincipal VerifyToken(string token)

            {
                if (token == null)
                {
                    throw new ArgumentNullException(nameof(token), "Token cannot be null.");
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("OUR_SECRET_KEY_FROM_LIFE_FROM_GJIRAFA"); 
                try
                {
                    var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    }, out SecurityToken validatedToken);

                    return principal;
                }
                catch
                {
                    return null;
                }
            }
        
    }

}
