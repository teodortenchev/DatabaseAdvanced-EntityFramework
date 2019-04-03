using AutoMapper;
using ProductShop.Dtos;
using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Export Product

            CreateMap<Product, ExportProductDto>()
                .ForMember(x => x.Seller, y => y.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            //Buyers
            CreateMap<User, UserWithSalesDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(u => u.ProductsSold));
                
            CreateMap<Product, ProductDto>()
                .ForMember(x => x.BuyerFirstName, y => y.MapFrom(p => p.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, y => y.MapFrom(p => p.Buyer.LastName));

            //User - Products
            CreateMap<User, UserDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(u => u.ProductsSold));



            CreateMap<Product, SoldProductDto>();
               

            CreateMap<User, UserProductDto>();

            CreateMap<List<UserDto>, UserProductDto>()
                .ForMember(x => x.Users, y => y.MapFrom(obj => obj));


        }
    }
}
