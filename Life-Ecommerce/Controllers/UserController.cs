using Microsoft.AspNetCore.Mvc;
using Application.Services.UserRepository;
using Domain.DTOs.User;
using Domain.Entities;

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
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            var userRole = HttpContext.Items["https://ecommerce-life-2.com/role"] as string;
            if (userRole == "SuperAdmin")
            {
                var users = await _userService.GetUsers(page, pageSize);
                return Ok(users);
            }
            return Unauthorized("You are not authorized to view this content");
        }
        
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> Put(UpdateUserDto user)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _userService.UpdateUser(token, user);
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
            
            var token = _userService.GenerateToken(user.Id, user.Role, user.Email);
            
            return Ok(new
            {
                IsAuthenticated = true,
                Role = user.Role,
                Token = token
            });
        }

        [HttpDelete]
        [Route("DeleteUser")] 
        public JsonResult Delete()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            _userService.DeleteUser(token);
            return new JsonResult("Deleted Successfully");
        }


        [HttpGet]
        [Route("GetUserByID/{Id}")]
        public async Task<IActionResult> GetUserByID(string Id)
        {
            var user = await _userService.GetUserById(Id);
            return Ok(user);
        }

        [HttpGet]
        [Route("GetUsersByRole")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _userService.GetUsersByRole(role);
            return Ok(users);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null || string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                return BadRequest("Invalid password change request.");
            }
            
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _userService.ChangePassword(token, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result)
            {
                return Ok("Password changed successfully.");
            }
            return BadRequest("Old password does not match or user not found.");
        }


    }
}
