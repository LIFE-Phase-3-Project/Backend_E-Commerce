using AutoMapper;
using Domain.DTOs.Category;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var category = await _unitOfWork.Repository<Domain.Entities.Category>().GetAll()
                .Include(sc => sc.Subcategories)
                .ToListAsync();
        
        return _mapper.Map<IEnumerable<CategoryDto>>(category);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
    {
        var category = await _unitOfWork.Repository<Domain.Entities.Category>().GetById(x => x.CategoryId == categoryId)
            .Include(sc => sc.Subcategories)
            .FirstOrDefaultAsync();
        
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task AddCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = _mapper.Map<Domain.Entities.Category>(categoryDto);
        _unitOfWork.Repository<Domain.Entities.Category>().Create(category);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateCategoryAsync(int id, CreateCategoryDto categoryDto)
    {
        var category = _mapper.Map<Domain.Entities.Category>(categoryDto);

        category.CategoryId = id;
            
        _unitOfWork.Repository<Domain.Entities.Category>().Update(category);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        var category = await _unitOfWork.Repository<Domain.Entities.Category>().GetById(x => x.CategoryId == categoryId).FirstOrDefaultAsync();
        if (category != null)
        {
            _unitOfWork.Repository<Domain.Entities.Category>().Delete(category);
            var result = await _unitOfWork.CompleteAsync();
            return result;
        }

        return false;
    }
}