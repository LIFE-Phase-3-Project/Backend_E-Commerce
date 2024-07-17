using AutoMapper;
using Domain.DTOs.Product;
using Domain.DTOs.Review;
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
        CreateMap<Product, ShoppingCartItemDto>().ReverseMap();
        CreateMap<Review, ReadReviewDto>()
            .ForMember(dest => dest.UserOverview, opt => opt.MapFrom(src => src.User));
        CreateMap<User, UserOverviewDto>().ReverseMap();
        }
    }
}
