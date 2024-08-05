using Application.Services.Search;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Nest;
using Domain.DTOs.Pagination;
using Application.Services.ImageStorage;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using Hangfire;



namespace Application.Services.Product;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISearchService _searchService;
    private readonly IElasticClient _elasticClient;
    private readonly IStorageService _storageService;
    private readonly ILogger<ProductService> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IElasticClient elasticClient, ISearchService searchService, IStorageService storageService, ILogger<ProductService> logger, IBackgroundJobClient backgroundJobClient)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _searchService = searchService;
        _elasticClient = elasticClient;
        _storageService = storageService;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<IEnumerable<ProductSearchDto>> SearchAsYouTypeAsync(string query)
    {
        var searchResults = await _searchService.SearchProductsAsYouType(query);

        var productSearchDtos = _mapper.Map<IEnumerable<ProductSearchDto>>(searchResults);

        return productSearchDtos;
    }
    public async Task<PaginatedInfo<ProductIndexDto>> GetPaginatedProductsAsync(
    ProductFilterModel filters,
    int page, int pageSize)
    {

        var searchResults = await _searchService.SearchProductsAsync(filters, page, pageSize);


        return new PaginatedInfo<ProductIndexDto>
        {
            Items = searchResults.Items.ToList(),
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
        _backgroundJobClient.Enqueue(() => LogProductRetrievalAsync(product.Id, product.CategoryId, product.SubCategoryId));
        return productDto;
    }

    public async Task AddProductAsync(CreateProductDto createProductDto)
    {
        var images = new List<string>();
       foreach (var item in createProductDto.Image)
        {
            var st = await _storageService.UploadFileAsync(item);
            images.Add(st);
        }
        var product = _mapper.Map<Domain.Entities.Product>(createProductDto);
        product.Image = images;
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

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByCondition(x => x.Id == id)
     .FirstOrDefaultAsync();

        if (product == null)
        {
            return false;
        }


        foreach (var imageUrl in product.Image)
        {
            var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
            await _storageService.DeleteFileAsync(fileName);
        }


        var newImageUrls = new List<string>();
        foreach (var image in updateProductDto.Image)
        {
            var imageUrl = await _storageService.UploadFileAsync(image);
            newImageUrls.Add(imageUrl);
        }

        _mapper.Map(updateProductDto, product);
        product.Image = newImageUrls;

        _unitOfWork.Repository<Domain.Entities.Product>().Update(product);
        var result = await _unitOfWork.CompleteAsync();
        var productToIndex = _mapper.Map<ProductIndexDto>(product);
        await _searchService.IndexProductAsync(productToIndex);
        if (result)
            return true;
        return false;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetById(x => x.Id == id).FirstOrDefaultAsync();

        if (product == null)
        {
            return false;
        }
        product.IsDeleted = true;
        _unitOfWork.Repository<Domain.Entities.Product>().Update(product);
        await _unitOfWork.CompleteAsync();
        await _searchService.DeleteProductFromIndexAsync(product.Id);

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
        // remove product from elastic search
         await _searchService.DeleteProductFromIndexAsync(productId);
    }

    //add method to add discount to product
    public async Task AddDiscountToProduct(int productId, decimal discount, DateTime ExpiryDate)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
        if (product != null)
        {
            product.DiscountPercentage = discount;
            product.DiscountExpiryDate = ExpiryDate; 
            _unitOfWork.Complete();
        }

    }

    public async Task LogProductRetrievalAsync(int productId, int categoryId, int subCategoryId)
    {
        await _elasticClient.IndexAsync(new ProductLog
        {
            ProductId = productId,
            CategoryId = categoryId,
            SubCategoryId = subCategoryId,
            RetrievedAt = DateTime.UtcNow
        }, idx => idx.Index("product_retrievals"));
    }
}