﻿using AutoMapper;
using Domain.DTOs.Product;
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
        }
    }
}
