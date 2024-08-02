using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.UserAddress;
using Domain.Entities;

namespace Application.Services.UserAddress
{
    public interface IUserAddressService
    {
        Task<IEnumerable<Domain.Entities.UserAddress>> GetUserAddresses(int userId);

        Task<int> CreateUserAddress(CreateUserAddressDto userAddress);

        Task<bool> MakeDefault(int addressId);

        Task<Domain.Entities.UserAddress> GetUserPrimaryAddress(int addressId);
        Task RemoveAddressFromProfile(int addressId);
        Task UpdateUserAddress(Domain.Entities.UserAddress userAddress);



    }
}
