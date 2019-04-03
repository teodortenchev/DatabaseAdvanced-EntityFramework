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

namespace CarDealer
{
    public class StartUp
    {

        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (CarDealerContext context = new CarDealerContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                DBInitializeFromJson(context);
            }
        }



        public static void DBInitializeFromJson(CarDealerContext context)
        {
            ImportSuppliers(context, File.ReadAllText(ImportFilesHelper.SuppliersJsonPath));
            ImportParts(context, File.ReadAllText(ImportFilesHelper.PartsJsonPath));
            string result = ImportCars(context, File.ReadAllText(ImportFilesHelper.CarsJsonPath));
            ImportCustomers(context, File.ReadAllText(ImportFilesHelper.CarsJsonPath));
            ImportSales(context, File.ReadAllText(ImportFilesHelper.SalesJsonPath));

            Console.WriteLine(result);
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
            var carsImport = JsonConvert.DeserializeObject<CarDto[]>(inputJson);

            var readyCars = Mapper.Map<CarDto[], Car[]>(carsImport);
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

            HashSet<int> customerIds = context.Customers.Select(x => x.Id).ToHashSet();
            HashSet<int> carIds = context.Cars.Select(x => x.Id).ToHashSet();

            List<Sale> verifiedSales = new List<Sale>();

            foreach (var sale in salesImport)
            {
                int customerId = sale.CustomerId;
                int carId = sale.CarId;

                if (!customerIds.Contains(customerId) && !carIds.Contains(carId))
                {
                    Console.WriteLine("Invalid Id found.");
                    continue;
                }

                verifiedSales.Add(sale);
            }

            context.Sales.AddRange(verifiedSales);

            int rowsCount = context.SaveChanges();

            return $"Successfully imported {rowsCount}.";
        }









    }
}