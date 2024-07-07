using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.UserRepository;
using Domain.User;
using Microsoft.AspNetCore.Authorization;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserRepository userRepository;

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(User u)
        {
             await userRepository.AddUser(u);
            return Ok(u); 

        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> Get()
        {
            var users = await userRepository.GetUsers();
            return Ok(users);
        }
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> Put(User user)
        {
            await userRepository.UpdateUser(user);
            return Ok("Updated Successfully");
        }

        [HttpDelete]
        [Route("DeleteUser")]  
        public JsonResult Delete(int id)
        {
            userRepository.DeleteUser(id);
            return new JsonResult("Deleted Successfully");
        }


        [HttpGet]
        [Route("GetUserByID/{Id}")]
        public async Task<IActionResult> GetUserByID(int Id)
        {
            return Ok(await userRepository.GetUserById(Id));
        }

        [HttpGet]
        [Route("GetUsersByRoleId")]
        public async Task<IActionResult> GetUsersByRoleId(int roleId)
        {
            var users = await userRepository.GetUsersByRoleId(roleId);
            return Ok(users);
        }


    }
}
