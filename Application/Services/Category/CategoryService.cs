using AutoMapper;
using Domain.DTOs.Category;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Newtonsoft;
using Newtonsoft.Json;

namespace Application.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    // add redis connection here
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper,  IConnectionMultiplexer connectionMultiplexer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redis = connectionMultiplexer;
        _db = _redis.GetDatabase();

    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        if (await _db.KeyExistsAsync("AllCategories"))
        {
            var categories = await _db.StringGetAsync("AllCategories");
            await _db.KeyExpireAsync("AllCategories", TimeSpan.FromMinutes(20));
            return JsonConvert.DeserializeObject<IEnumerable<CategoryDto>>(categories);
        }
        var category = await _unitOfWork.Repository<Domain.Entities.Category>().GetAll()
                .Include(sc => sc.Subcategories)
                .ToListAsync();
        await _db.StringSetAsync("AllCategories", JsonConvert.SerializeObject(_mapper.Map<IEnumerable<CategoryDto>>(category)), TimeSpan.FromMinutes(20));
        
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
        await _db.KeyExpireAsync("AllCategories", TimeSpan.FromMinutes(0)); // expire the cache
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