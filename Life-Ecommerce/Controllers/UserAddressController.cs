using Application.Services.UserAddress;
using Domain.DTOs.UserAddress;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;

        public UserAddressController(IUserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserAddresses(int userId)
        {
            var userAddresses = await _userAddressService.GetUserAddresses(userId);
            return Ok(userAddresses);
        }

        [HttpGet("primary")]
        public async Task<IActionResult> GetUserPrimaryAddress(int userId)
        {
            var userPrimaryAddress = await _userAddressService.GetUserPrimaryAddress(userId);
            return Ok(userPrimaryAddress);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress(CreateUserAddressDto userAddress)
        {
            var result = await _userAddressService.CreateUserAddress(userAddress);
            return Ok(result);
        }

        [HttpPut("make-default/{addressId}")]
        public async Task<IActionResult> MakeDefault(int addressId)
        {
            var result = await _userAddressService.MakeDefault(addressId);
            return Ok(result);
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> RemoveAddressFromProfile(int addressId)
        {
            await _userAddressService.RemoveAddressFromProfile(addressId);
            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserAddress(Domain.Entities.UserAddress userAddress)
        {
            await _userAddressService.UpdateUserAddress(userAddress);
            return Ok();
        }


    }
}
