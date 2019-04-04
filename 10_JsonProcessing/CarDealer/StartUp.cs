using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Imports;
using CarDealer.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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
                //DBInitializeFromJson(context);

                string result = GetCarsWithTheirListOfParts(context);

                Console.WriteLine(result);
            }
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            List<Customer> orderedCustomers = context.Customers.OrderBy(x => x.BirthDate).ThenBy(x => x.IsYoungDriver).ToList();

            var customerDtos = Mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerDto>>(orderedCustomers);

            string jsonOutput = JsonConvert.SerializeObject(customerDtos, Formatting.Indented);

            return jsonOutput;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            List<Car> toyotaCars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();

            List<CarExportDto> exportedCars = Mapper.Map<List<Car>, List<CarExportDto>>(toyotaCars);

            string jsonOutput = JsonConvert.SerializeObject(exportedCars, Formatting.Indented);

            return jsonOutput;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var domesticSuppliers = context.Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                }).ToList();


            string jsonOutput = JsonConvert.SerializeObject(domesticSuppliers, Formatting.Indented);

            return jsonOutput;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars.Include(x => x.PartCars).ThenInclude(p => p.Part).ToList()
                .Select(x => new
                {
                    car = new
                    {
                        x.Make,
                        x.Model,
                        x.TravelledDistance
                    },
                    parts = x.PartCars.Select(pc => new
                    {
                        pc.Part.Name,
                        Price = $"{pc.Part.Price:F2}"
                    })
                })
                .ToList();

            string jsonOutput = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);

            return jsonOutput;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customersWithPurchases = context.Customers
                .Where(x => x.Sales.Count > 0)
                .Include(x => x.Sales)
                .ThenInclude(s => s.Car)
                .ThenInclude(s => s.PartCars)
                .ToList()
                .Select(x => new
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = $"{x.Sales.Sum(s => s.Car.PartCars.Sum(b => b.Part.Price)):F2}"
                });
        }

        public static void DBInitializeFromJson(CarDealerContext context)
        {
            ImportSuppliers(context, File.ReadAllText(ImportFilesHelper.SuppliersJsonPath));
            ImportParts(context, File.ReadAllText(ImportFilesHelper.PartsJsonPath));
            ImportCars(context, File.ReadAllText(ImportFilesHelper.CarsJsonPath));
            ImportCustomers(context, File.ReadAllText(ImportFilesHelper.CustomersJsonPath));
            ImportSales(context, File.ReadAllText(ImportFilesHelper.SalesJsonPath));

            Console.WriteLine("DB initialized successfully with sample data from Datasets folder.");
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersImport = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.AddRange(suppliersImport);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var partsImport = JsonConvert.DeserializeObject<List<Part>>(inputJson);

            HashSet<int> supplierIds = context.Suppliers.Select(x => x.Id).ToHashSet();

            List<Part> validParts = new List<Part>();

            foreach (var part in partsImport)
            {
                int supplierId = part.SupplierId;

                if (!supplierIds.Contains(supplierId))
                {
                    continue;
                }

                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsImport = JsonConvert.DeserializeObject<CarImportDto[]>(inputJson);

            var readyCars = Mapper.Map<CarImportDto[], Car[]>(carsImport);
            context.AddRange(readyCars);

            context.SaveChanges();

            HashSet<int> partIds = context.Parts.Select(x => x.Id).ToHashSet();
            HashSet<Part> parts = context.Parts.ToHashSet();
            HashSet<Car> cars = context.Cars.ToHashSet();
            HashSet<PartCar> addedCarParts = new HashSet<PartCar>();

            List<PartCar> partsList = new List<PartCar>();

            foreach (var car in carsImport)
            {
                car.PartsId = car.PartsId.Distinct().ToList();

                Car currentCar = cars.Where(x => x.Make == car.Make && x.Model == car.Model && x.TravelledDistance == car.TravelledDistance).FirstOrDefault();

                if (currentCar == null)
                {
                    continue;
                }

                foreach (var id in car.PartsId)
                {
                    if (!partIds.Contains(id))
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar
                    {
                        CarId = currentCar.Id,
                        PartId = id
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

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            List<Customer> customersImport = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customersImport);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            List<Sale> salesImport = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            

            context.Sales.AddRange(salesImport);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }









    }
}