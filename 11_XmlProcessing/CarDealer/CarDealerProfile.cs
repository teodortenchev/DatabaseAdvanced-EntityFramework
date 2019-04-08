using AutoMapper;
using CarDealer.Dtos.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportCarDto, Car>();
            CreateMap<ImportCustomerDto, Customer>();
            CreateMap<ImportPartDto, Part>();
            CreateMap<ImportSaleDto, Sale>();
            CreateMap<ImportSupplierDto, Supplier>();
        }
    }
}
