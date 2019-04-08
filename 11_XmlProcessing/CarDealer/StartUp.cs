using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{

    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (CarDealerContext context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                //DBInitializeFromXml(context);
            }


        }


        //Exports
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var exportCars = context.Cars
                .Where(c => c.TravelledDistance >= 2000000)
                .Select(x => new ExportCarWithDistanceDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarWithDistanceDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new  XmlSerializerNamespaces(XmlQualifiedName.Empty)


        }



        //Db Seeding / Imports

        public static void DBInitializeFromXml(CarDealerContext context)
        {
            ImportSuppliers(context, XmlToString("../../../Datasets/suppliers.xml"));
            ImportParts(context, XmlToString("../../../Datasets/parts.xml"));
            ImportCars(context, XmlToString("../../../Datasets/cars.xml"));
            ImportCustomers(context, XmlToString("../../../Datasets/customers.xml"));
            ImportSales(context, XmlToString("../../../Datasets/sales.xml"));

        }

        private static string XmlToString(string xml)
        {
            return File.ReadAllText(xml, Encoding.UTF8);
        }


        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));

            var dtos = (ImportSupplierDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = Mapper.Map<ImportSupplierDto[], Supplier[]>(dtos);

            context.Suppliers.AddRange(suppliers);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));

            HashSet<int> supplierIds = context.Suppliers.Select(x => x.Id).ToHashSet();

            var dtos = (ImportPartDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var verifiedDtos = new List<ImportPartDto>();

            foreach (var item in dtos)
            {
                if (!supplierIds.Contains(item.SupplierId))
                {
                    continue;
                }

                verifiedDtos.Add(item);
            }

            var parts = Mapper.Map<List<ImportPartDto>, List<Part>>(verifiedDtos);

            context.Parts.AddRange(parts);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}";


        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));

            var dtos = (ImportCarDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var carsImport = Mapper.Map<ImportCarDto[], Car[]>(dtos);

            context.Cars.AddRange(carsImport);

            context.SaveChanges();

            HashSet<int> partIds = context.Parts.Select(x => x.Id).ToHashSet();
            HashSet<Part> parts = context.Parts.ToHashSet();
            HashSet<Car> cars = context.Cars.ToHashSet();
            HashSet<PartCar> addedCarParts = new HashSet<PartCar>();

            List<PartCar> partsList = new List<PartCar>();

            foreach (var car in dtos)
            {
                car.PartIds = car.PartIds.Distinct().ToList();


                Car currentCar = cars.Where(x => x.Make == car.Make && x.Model == car.Model && x.TravelledDistance == car.TravelledDistance).FirstOrDefault();

                if (currentCar == null)
                {
                    continue;
                }

                foreach (var id in car.PartIds)
                {
                    int pId = id.Id;
                    if (!partIds.Contains(pId))
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar
                    {
                        CarId = currentCar.Id,
                        PartId = pId
                    };

                    if (!addedCarParts.Contains(partCar))
                    {
                        partsList.Add(partCar);
                        addedCarParts.Add(partCar);
                    }

                }

                if (partsList != null)
                {
                    currentCar.PartCars = partsList;
                    context.PartCars.AddRange(partsList);
                    partsList.Clear();
                }
            }

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {cars.Count}";

        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var dtos = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = Mapper.Map<ImportCustomerDto[], Customer[]>(dtos);

            context.AddRange(customers);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));

            var dtos = (ImportSaleDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            //HashSet<int> customerIds = context.Customers.Select(x => x.Id).ToHashSet();
            HashSet<int> carIds = context.Cars.Select(x => x.Id).ToHashSet();


            var verifiedSales = new List<ImportSaleDto>();

            foreach (var item in dtos)
            {
                if (!carIds.Contains(item.CarId))
                {
                    continue;
                }

                verifiedSales.Add(item);

            }

            var sales = Mapper.Map<List<ImportSaleDto>, List<Sale>>(verifiedSales);

            context.Sales.AddRange(sales);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}";

        }
    }
}