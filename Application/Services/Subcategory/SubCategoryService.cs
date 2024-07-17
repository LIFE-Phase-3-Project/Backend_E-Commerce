using AutoMapper;
using Domain.DTOs.SubCategory;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Subcategory;

public class SubCategoryService : ISubCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubCategoryDto>> GetAllSubCategoriesAsync()
        {
            var subCategories = await _unitOfWork.Repository<SubCategory>().GetAll()
                .Include(sc => sc.Category)
                .ToListAsync();
            return _mapper.Map<IEnumerable<SubCategoryDto>>(subCategories);
        }

        public async Task<SubCategoryDto> GetSubCategoryByIdAsync(int subCategoryId)
        {
            var subCategory = await _unitOfWork.Repository<SubCategory>().GetById(sc => sc.SubCategoryId == subCategoryId)
                .Include(sc => sc.Category)
                .FirstOrDefaultAsync();

            if (subCategory == null)
            {
                throw new KeyNotFoundException("SubCategory not found");
            }

            return _mapper.Map<SubCategoryDto>(subCategory);
        }

        public async Task<SubCategoryDto> AddSubCategoryAsync(CreateSubCategoryDto subCategoryDto)
        {
            var subCategory = _mapper.Map<SubCategory>(subCategoryDto);
            _unitOfWork.Repository<SubCategory>().Create(subCategory);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SubCategoryDto>(subCategory);
        }

        public async Task UpdateSubCategoryAsync(int id, UpdateSubCategoryDto subCategoryDto)
        {
            var subCategory = _mapper.Map<SubCategory>(subCategoryDto);
            
            subCategory.SubCategoryId = id;
            
            _unitOfWork.Repository<SubCategory>().Update(subCategory);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeleteSubCategoryAsync(int subCategoryId)
        {
            var subCategory = await _unitOfWork.Repository<SubCategory>().GetById(sc => sc.SubCategoryId == subCategoryId).FirstOrDefaultAsync();
            if (subCategory == null)
            {
                return false;
            }

            _unitOfWork.Repository<SubCategory>().Delete(subCategory);
            await _unitOfWork.CompleteAsync();
            return true;
        }
}