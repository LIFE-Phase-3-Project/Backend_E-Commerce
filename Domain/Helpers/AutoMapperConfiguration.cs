using AutoMapper;
using Domain.DTOs.Category;
using Domain.DTOs.Order;
using Domain.DTOs.Payment;
using Domain.DTOs.Product;
using Domain.DTOs.Review;
using Domain.DTOs.SubCategory;
using Domain.DTOs.User;
using Domain.Entities;
namespace Domain.Helpers
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration() { 
            CreateMap<User, RegisterUserDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Review, CreateReviewDto>().ReverseMap();

            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserOverview, opt => opt.MapFrom(src => src.User));
        
            CreateMap<User, UserOverviewDto>().ReverseMap();
            CreateMap<User, UserWithRoleDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap()
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories));
        
            CreateMap<Category, CategoryDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<SubCategory, SubCategoryDto>();
                // .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));
        
            CreateMap<CreateSubCategoryDto, SubCategory>();
            CreateMap<UpdateSubCategoryDto, SubCategory>();
            CreateMap<ChangePasswordDto, User>();
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderDto, Order>()
             .ReverseMap();

            CreateMap<MonthlyPaymentDto, Payment>();
            CreateMap<CreatePaymentDto, Payment>();
            CreateMap<CreateCashPaymentDto, Payment>();
        }
        
    }
}
