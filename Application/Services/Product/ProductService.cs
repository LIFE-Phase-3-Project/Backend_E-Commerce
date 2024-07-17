using AutoMapper;
using Domain.DTOs.Product;
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
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByCondition(p => p.Id == id)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            // Handle product not found
            return null;
        }

        return _mapper.Map<ProductDto>(product);
    }

    public async Task AddProductAsync(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Domain.Entities.Product>(createProductDto);

        _unitOfWork.Repository<Domain.Entities.Product>().Create(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateProductAsync(int id, CreateProductDto updateProductDto)
    {
        var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByCondition(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            // Handle product not found
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
            // Handle product not found
            return false;
        }

        _unitOfWork.Repository<Domain.Entities.Product>().Delete(product);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}