using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Imports;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        
        public static void Main(string[] args)
        {
            using(CarDealerContext context = new CarDealerContext())
            {
                context.Database.EnsureCreated();
            }
        }



        public static void DBInitializeFromJson(CarDealerContext context)
        {
            ImportSuppliers(context, ImportFilesHelper.SuppliersJsonPath);
            ImportParts(context, ImportFilesHelper.PartsJsonPath);
            ImportCars(context, ImportFilesHelper.CarsJsonPath);
            ImportCustomers(context, ImportFilesHelper.CustomersJsonPath);
            ImportSales(context, ImportFilesHelper.SalesJsonPath);

        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            return $"Successfully imported {recordsCount}.";
        }

        public static string ImportParts(CarDealerContext context, string partsJsonPath)
        {
            throw new NotImplementedException();
        }

        public static string ImportCars(CarDealerContext context, string partsJsonPath)
        {
            throw new NotImplementedException();
        }

        public static string ImportCustomers(CarDealerContext context, string partsJsonPath)
        {
            throw new NotImplementedException();
        }
        
        public static string ImportSales(CarDealerContext context, string partsJsonPath)
        {
            throw new NotImplementedException();
        }

        

        


        

        
    }
}