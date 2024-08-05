using Application.Services.TokenService;
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
        private readonly TokenHelper _tokenHelper;

        public UserAddressController(IUserAddressService userAddressService, TokenHelper tokenHelper)
        {
            _userAddressService = userAddressService;
            _tokenHelper = tokenHelper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = _tokenHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized("You are not logged in.");
            }
            var userAddresses = await _userAddressService.GetUserAddresses(userId);
            return Ok(userAddresses);
        }

        [HttpGet("primary")]
        public async Task<IActionResult> GetUserPrimaryAddress()
        {
            var userId = _tokenHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized("You are not logged in.");
            }
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
