using Microsoft.AspNetCore.Mvc;
using Presistence;
using Application.Services.UserRepository;
using Domain.DTOs.User;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(RegisterUserDto registerUserDto)
        {
            await _userService.AddUser(registerUserDto);
            return Ok(registerUserDto); 
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
        public IActionResult Login([FromBody] UserLoginDto request)
        {
            var user = _userService.AuthenticateUser(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized("Email or password is incorrect");
            }

            var roleName = _userService.GetUserRole(user.RoleId);
            var email = request.Email;

            var token = _userService.GenerateToken(user.Id, roleName, email);
            
            return Ok(new
            {
                IsAuthenticated = true,
                Role = roleName,
                Token = token
            });
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
            var user = await _userService.GetUserById(Id);
            return Ok(user);
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
