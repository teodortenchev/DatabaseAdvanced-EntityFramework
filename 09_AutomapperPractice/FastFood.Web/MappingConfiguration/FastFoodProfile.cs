namespace FastFood.Web.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Web.ViewModels.Categories;
    using FastFood.Web.ViewModels.Employees;
    using FastFood.Web.ViewModels.Items;
    using FastFood.Web.ViewModels.Orders;
    using Models;
    using System;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Employees
            this.CreateMap<RegisterEmployeeInputModel, Employee>();

            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionName, y => y.MapFrom(p => p.Name));

            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(x => x.Position, y => y.MapFrom(e => e.Position.Name));

            //Categories
            this.CreateMap<CreateCategoryInputModel, Category>()
                 .ForMember(x => x.Name, y => y.MapFrom(c => c.CategoryName));

            this.CreateMap<Category, CategoryAllViewModel>();

            //Items'x => x.Order.Customer
            this.CreateMap<Category, CreateItemViewModel>()
                .ForMember(x => x.CategoryId, y => y.MapFrom(c => c.Id));

            this.CreateMap<CreateItemInputModel, Item>();

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(x => x.Category, y => y.MapFrom(i => i.Category.Name));

            //Orders
            this.CreateMap<CreateOrderInputModel, Order>()
                .ForMember(x => x.Type, y => y.MapFrom(o => o.Type));



            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.OrderId, y => y.MapFrom(o => o.Id))
                .ForMember(x => x.DateTime, y => y.MapFrom(o => o.DateTime.ToString("g")))
                .ForMember(x => x.Employee, y => y.MapFrom(o => o.Employee.Name));





        }
    }
}
