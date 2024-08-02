using AutoMapper;
using Domain.DTOs.UserAddress;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Services.UserAddress
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserAddressService(IUnitOfWork unitOfWork, IMapper mapper)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<int> CreateUserAddress(CreateUserAddressDto userAddress)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(userAddress);
            if (!Validator.TryValidateObject(userAddress, validationContext, validationResults, true))
            {
                throw new ValidationException("User address validation failed.");
            }
            var address = _mapper.Map<Domain.Entities.UserAddress>(userAddress);
            _unitOfWork.Repository<Domain.Entities.UserAddress>().Create(address);
            await _unitOfWork.CompleteAsync();
            return address.Id;
        }
        public async Task<bool> MakeDefault(int addressId)
        {
            var address = await _unitOfWork.Repository<Domain.Entities.UserAddress>().GetByIdAsync(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException("Address Not Found");
            }
            var primaryAddress = await _unitOfWork.Repository<Domain.Entities.UserAddress>().GetByCondition(x => x.IsPrimary).FirstOrDefaultAsync();
            if (primaryAddress != null)
            {
                primaryAddress.IsPrimary = false;
                _unitOfWork.Repository<Domain.Entities.UserAddress>().Update(primaryAddress);
            }
            address.IsPrimary = true;
            _unitOfWork.Repository<Domain.Entities.UserAddress>().Update(address);
            await _unitOfWork.CompleteAsync();
            return true;

        }

        public async Task<IEnumerable<Domain.Entities.UserAddress>> GetUserAddresses(int userId)
        {
            var addresses =  await _unitOfWork.Repository<Domain.Entities.UserAddress>().GetAll().Where(x => x.UserId == userId && x.IsActive).ToListAsync();
            return addresses;
        }

        public async Task<Domain.Entities.UserAddress> GetUserPrimaryAddress(int userId)
        {
            var address = await _unitOfWork.Repository<Domain.Entities.UserAddress>().GetByCondition(x => x.IsPrimary && x.UserId == userId).FirstOrDefaultAsync();
            return address;
        }

        public async Task UpdateUserAddress(Domain.Entities.UserAddress userAddress)
        {
            _unitOfWork.Repository<Domain.Entities.UserAddress>().Update(userAddress);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveAddressFromProfile(int addressId)
        {
            var address = await _unitOfWork.Repository<Domain.Entities.UserAddress>().GetByIdAsync(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException("Address Not Found");
            }
            address.IsActive = false;
            address.IsPrimary = false;
            _unitOfWork.Repository<Domain.Entities.UserAddress>().Update(address);
            await _unitOfWork.CompleteAsync();
        }

    }
}
