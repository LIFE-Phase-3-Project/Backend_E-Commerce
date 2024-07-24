// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Mvc;
// using System.Security.Claims;
// using Application;
// using Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using System.IdentityModel.Tokens.Jwt;
//
// namespace Life_Ecommerce.Controllers
// {
//     [Route("[controller]")]
//     [ApiController]
//     public class AuthController : ControllerBase
//     {
//         private readonly IUnitOfWork _unitOfWork;
//
//         public AuthController(IUnitOfWork unitOfWork)
//         {
//             _unitOfWork = unitOfWork;
//         }
//
//         [HttpGet("google-action")]
//         public async Task<IActionResult> GoogleAction()
//         {
//             // Check if the user is already logged in
//             var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//             if (result.Succeeded)
//             {
//                 // User is authenticated, redirect to Google callback
//                 var callbackUrl = Url.Action(nameof(GoogleLoginCallback), "Auth");
//                 return Redirect(callbackUrl);
//             }
//             else
//             {
//                 // User is not authenticated, redirect to Google sign-in
//                 var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Auth");
//                 var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
//                 return Challenge(properties, "Google");
//             }
//         }
//
//         [HttpGet("google-callback")]
//         public async Task<IActionResult> GoogleLoginCallback()
//         {
//             var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//
//             var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
//             if (email == null)
//             {
//                 return BadRequest("Email is required");
//             }
//
//             var user = await _unitOfWork.Repository<User>()
//                 .GetByCondition(x => x.Email == email)
//                 .Include(x => x.UserRole)
//                 .FirstOrDefaultAsync();
//
//             if (user == null)
//             {
//                 string address = authenticateResult.Principal.FindFirstValue(ClaimTypes.Country + " " + ClaimTypes.StreetAddress + " " + ClaimTypes.PostalCode) ?? "Default Address";
//                 var phoneNumber = authenticateResult.Principal.FindFirstValue(ClaimTypes.MobilePhone + " " + ClaimTypes.HomePhone + " " + ClaimTypes.OtherPhone + " " + ClaimTypes.OtherPhone)
//                                 ?? "123";
//                 // var phoneNumberr = User.FindFirst("urn:google:phone")?.Value;
//
//                 var profilePictureUrl = authenticateResult.Principal.FindFirstValue("urn:google:picture");
//
//                 user = new User
//                 {
//                     Email = email,
//                     FirstName = authenticateResult.Principal.FindFirstValue(ClaimTypes.GivenName),
//                     LastName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Surname),
//                     Password = "string", // Placeholder, not used for Google users
//                     RoleId = 1, // Assuming default role
//                     Address = address,
//                     PhoneNumber = phoneNumber,
//                 };
//                 _unitOfWork.Repository<User>().Create(user);
//                 await _unitOfWork.CompleteAsync();
//
//                 // Reload the user to get the role information
//                 user = await _unitOfWork.Repository<User>()
//                     .GetByCondition(x => x.Email == email)
//                     .Include(x => x.UserRole)
//                     .FirstOrDefaultAsync();
//             }
//
//             var roleName = user.UserRole?.RoleName;
//             if (roleName == null)
//             {
//                 return BadRequest("Role does not exist");
//             }
//
//             var token = TokenService.TokenService.GenerateToken(user.Id, roleName, email);
//
//             var frontendUrl = $"http://localhost:3000/auth/callback?token={token}";
//             return Redirect(frontendUrl);
//         }
//
//         [HttpPost("logout")]
//         public async Task<IActionResult> Logout()
//         {
//             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//             return Ok("User logged out successfully.");
//         }
//
//         [HttpGet("get-token")]
//         public async Task<IActionResult> GetToken()
//         {
//             var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//             if (!result.Succeeded)
//             {
//                 return Unauthorized("User is not authenticated");
//             }
//
//             var email = result.Principal.FindFirstValue(ClaimTypes.Email);
//             if (email == null)
//             {
//                 return BadRequest("Email is required");
//             }
//
//             var user = await _unitOfWork.Repository<User>()
//                 .GetByCondition(x => x.Email == email)
//                 .Include(x => x.UserRole)
//                 .FirstOrDefaultAsync();
//
//             if (user == null)
//             {
//                 return Unauthorized("User not found");
//             }
//
//             var roleName = user.UserRole?.RoleName;
//             if (roleName == null)
//             {
//                 return BadRequest("Role does not exist");
//             }
//
//             var token = TokenService.TokenService.GenerateToken(user.Id, roleName, email);
//
//             return Ok(new { token });
//         }
//     }
// }
