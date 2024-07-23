using Microsoft.AspNetCore.Mvc;
using Presistence;
using Application.Services.UserRepository;
using Domain.DTOs.User;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Application.Services.ShoppingCart;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly IShoppingCartService _shoppingCartService;
        public readonly APIDbContext _context;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public UserController(IUserService userService, APIDbContext con, IDataProtectionProvider iDataProtectionProvider, IShoppingCartService shoppingCartService)
        {
            _userService = userService;
            _context = con;
            _dataProtectionProvider = iDataProtectionProvider;
            _shoppingCartService = shoppingCartService;
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(RegisterUserDto u)
        {
            await _userService.AddUser(u);
            return Ok(u); 

        }

        [HttpGet]
        [Route("GetUsers")]
       
        public async Task<IActionResult> Get()
        {
            var userRole = HttpContext.Items["UserRole"] as string;
            if (userRole == "SuperAdmin")
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            return Unauthorized("You are not authorized to view this content");

        }
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> Put(User user)
        {
            await _userService.UpdateUser(user);
            return Ok("Updated Successfully");
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserLogin Request)
        {
            var user = _context.Users.FirstOrDefault(user => user.Email == Request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(Request.Password, user.Password))
            {
                return Unauthorized("Mejli ose fjalkalimi eshte gabim");
            }

            var roleName = _userService.GetUserRole(user.RoleId);
            var email = Request.Email;

            var token = TokenService.TokenService.GenerateToken(user.Id, roleName, email);
            string encryptedCartIdentifier;
            if (HttpContext.Request.Cookies.TryGetValue("CartIdentifier", out encryptedCartIdentifier))
            {
                var unprotectedCartIdentifier = _dataProtectionProvider.CreateProtector("CartIdentifierProtector").Unprotect(encryptedCartIdentifier);
                await _shoppingCartService.MergeGuestCart(unprotectedCartIdentifier, user.Id);
                HttpContext.Response.Cookies.Delete("CartIdentifier");

            }
            return Ok(new
            {
                IsAuthenticated = true,
                Role = roleName,
                Token = token
            });
        }

        public class UserLogin
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpDelete]
        [Route("DeleteUser")] 
        public JsonResult Delete(int id)
        {
            _userService.DeleteUser(id);
            return new JsonResult("Deleted Successfully");
        }


        [HttpGet]
        [Route("GetUserByID/{Id}")]
        public async Task<IActionResult> GetUserByID(int Id)
        {
            return Ok(await _userService.GetUserById(Id));
        }

        [HttpGet]
        [Route("GetUsersByRoleId")]
        public async Task<IActionResult> GetUsersByRoleId(int roleId)
        {
            var users = await _userService.GetUsersByRoleId(roleId);
            return Ok(users);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null || string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                return BadRequest("Invalid password change request.");
            }

            var result = await _userService.ChangePassword(changePasswordDto.UserId, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result)
            {
                return Ok("Password changed successfully.");
            }
            else
            {
                return BadRequest("Old password does not match or user not found.");
            }
        }


    }
}
