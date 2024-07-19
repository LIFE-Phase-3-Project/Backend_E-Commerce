using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Product;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Repository<Domain.Entities.Product>().GetAll()
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByCondition(p => p.Id == id)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return null;
        }

        return _mapper.Map<ProductDto>(product);
    }

    public async Task AddProductAsync(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Domain.Entities.Product>(createProductDto);
        
        //get subcategory to autoAssign category
        var subcategory = _unitOfWork.Repository<SubCategory>()
            .GetByCondition(x => x.SubCategoryId == product.SubCategoryId).FirstOrDefault();
        
        if (subcategory == null)
        {
            throw new InvalidOperationException($"SubCategory with ID {product.SubCategoryId} does not exist.");
        }
        
        product.CategoryId = subcategory.CategoryId;

        _unitOfWork.Repository<Domain.Entities.Product>().Create(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateProductAsync(int id, CreateProductDto updateProductDto)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByCondition(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return;
        }

         _mapper.Map(updateProductDto, product);

        _unitOfWork.Repository<Domain.Entities.Product>().Update(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetById(x => x.Id == id).FirstOrDefaultAsync();

        if (product == null)
        {
            return false;
        }

        _unitOfWork.Repository<Domain.Entities.Product>().Delete(product);
        await _unitOfWork.CompleteAsync();
        return true;
    }
    
    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(int categoryId)
    {
        var products = await _unitOfWork.Repository<Domain.Entities.Product>()
            .GetByCondition(p => p.CategoryId == categoryId)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsBySubCategoryIdAsync(int subCategoryId)
    {
        var products = await _unitOfWork.Repository<Domain.Entities.Product>()
            .GetByCondition(p => p.SubCategoryId == subCategoryId)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
    public async void SoftDeleteProduct(int productId)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
        if (product != null)
        {
            product.IsDeleted = true;
            _unitOfWork.Complete();
        }
    }
}