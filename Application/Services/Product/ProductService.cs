using Application.Services.Search;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Nest;
using Domain.DTOs.Pagination;


namespace Application.Services.Product;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISearchService _searchService;
    private readonly IElasticClient _elasticClient;
    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IElasticClient elasticClient, ISearchService searchService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _searchService = searchService;
        _elasticClient = elasticClient;
    }

    public async Task<bool> TestElasticsearchConnectionAsync()
    {
        var response = await _elasticClient.Cluster.HealthAsync();
        var indexExists = await _elasticClient.Indices.ExistsAsync("products");
        if (response.IsValid && indexExists.Exists)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<PaginatedInfo<ProductDto>> GetPaginatedProductsAsync(
    ProductFilterModel filters,
    int page, int pageSize)
    {
        // Call the search service to get the products based on the search term
        var searchResults = await _searchService.SearchProductsAsync(filters, page, pageSize);

        // Map the search results to ProductDto
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(searchResults.Items);

        // Return the paginated info
        return new PaginatedInfo<ProductDto>
        {
            Items = productDtos.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = searchResults.TotalCount
        };
    }


    public async Task<PaginatedInfo<ProductDto>> GetPaginatedProductsAsync(
    Expression<Func<Domain.Entities.Product, bool>> filter,
    int page, int pageSize)
    {
        var query = _unitOfWork.Repository<Domain.Entities.Product>()
            .GetByCondition(filter)
            .Include(p => p.Reviews)
            .Include(p => p.Category);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(items);

        return new PaginatedInfo<ProductDto>
        {
            Items = productDtos.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
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
        var productDto = _mapper.Map<ProductDto>(product);
        return productDto;
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
        var productToIndex = _mapper.Map<ProductIndexDto>(product);
        await _searchService.IndexProductAsync(productToIndex);
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto updateProductDto)
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
        var productToIndex = _mapper.Map<ProductIndexDto>(product);
        await _searchService.IndexProductAsync(productToIndex);

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
    
    public async Task<PaginatedInfo<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int page, int pageSize)
    {
        return await GetPaginatedProductsAsync(p => p.CategoryId == categoryId, page, pageSize);


    }

    public async Task<PaginatedInfo<ProductDto>> GetProductsBySubCategoryIdAsync(int subCategoryId, int page, int pageSize)
    {
        return await GetPaginatedProductsAsync(p => p.SubCategoryId == subCategoryId, page, pageSize);
    }
    public async Task SoftDeleteProduct(int productId)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
        if (product != null)
        {
            product.IsDeleted = true;
            _unitOfWork.Complete();
        }
    }
}