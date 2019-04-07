using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportUserDto, User>();
            CreateMap<ImportProductDto, Product>();
            CreateMap<ImportCategoryDto, Category>();
            CreateMap<ImportCategoryProductDto, CategoryProduct>();

            CreateMap<Product, ExportProductDto>()
                .ForMember(x => x.Buyer, y => y.MapFrom(b => $"{b.Buyer.FirstName} {b.Buyer.LastName}"));
                
        }
    }
}
