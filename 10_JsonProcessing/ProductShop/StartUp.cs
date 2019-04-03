using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Dtos;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace ProductShop
{
    public class StartUp
    {

        public static void Main(string[] args)
        {

            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());


            string result = string.Empty;

            //Data Imports from JSON. 
            string jsonFileUsers = File.ReadAllText(@"F:\C# Fundamentals\DB Advanced\Product Shop - Skeleton\ProductShop\Datasets\users.json");
            string jsonFileProducts = File.ReadAllText(@"F:\C# Fundamentals\DB Advanced\Product Shop - Skeleton\ProductShop\Datasets\products.json");
            string jsonFileCategories = File.ReadAllText(@"F:\C# Fundamentals\DB Advanced\Product Shop - Skeleton\ProductShop\Datasets\categories.json");
            string jsonFileCategoryProducts = File.ReadAllText(@"F:\C# Fundamentals\DB Advanced\Product Shop - Skeleton\ProductShop\Datasets\categories-products.json");

            using (ProductShopContext context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();


                //ImportUsers(context, jsonFileUsers);
                //ImportProducts(context, jsonFileProducts);
                //ImportCategories(context, jsonFileCategories);
                //ImportCategoryProducts(context, jsonFileCategoryProducts);

                result = GetUsersWithProducts(context);
            }

            Console.WriteLine(result);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //var usersWithProductBuyers = context.Users
            //    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
            //    .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
            //    .ProjectTo<UserDto>()
            //    .ToList();


            //var userProductDtos = Mapper.Map<UserProductDto>(usersWithProductBuyers);


            var filteredUsers = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(ps => ps.Buyer != null))
                
                .Select(x => new
                {
                    UsersCount = x.ProductsSold.Count(ps => ps.Buyer != null),
                    Users = new
                    {
                        LastName = x.LastName,
                        Age = x.Age,
                        SoldProducts = new
                        {
                            Count = x.ProductsSold.Count(ps => ps.Buyer != null),
                            Products = x.ProductsSold
                                .Where(ps => ps.Buyer != null)
                                .Select(p => new
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                }).ToList()
                        }
                    }
                })
                .ToList();
            
            DefaultContractResolver contractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() };

            var jsonResult = JsonConvert.SerializeObject(filteredUsers, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,

            });

            return jsonResult;

        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                 .OrderByDescending(c => c.CategoryProducts.Count)
                 .Select(x => new
                 {
                     Category = x.Name,
                     ProductsCount = x.CategoryProducts.Count,
                     AveragePrice = $"{x.CategoryProducts.Average(c => c.Product.Price):F2}",
                     TotalRevenue = $"{x.CategoryProducts.Sum(c => c.Product.Price)}"
                 })
                 .ToList();

            string json = JsonConvert.SerializeObject(categories,
               new JsonSerializerSettings()
               {
                   ContractResolver = new DefaultContractResolver()
                   {
                       NamingStrategy = new CamelCaseNamingStrategy(),
                   },

                   Formatting = Formatting.Indented
               }
           );

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            //var usersWithSales = context.Users
            //    .Where(u => u.ProductsSold.Count != 0 && u.ProductsSold.Any(ps => ps.Buyer != null))
            //    .OrderBy(x => x.LastName)
            //    .ThenBy(x => x.FirstName)
            //    .ToList()
            //    .Select(x => new
            //    {
            //        FirstName = x.FirstName,
            //        LastName = x.LastName,
            //        SoldProducts = x.ProductsSold.Where(u => u.Buyer != null).Select(p => new
            //        {
            //            Name = p.Name,
            //            Price = p.Price,
            //            BuyerFirstName = p.Buyer.FirstName,
            //            BuyerLastName = p.Buyer.LastName
            //        })

            //    })
            //    .ToList();


            var usersWithSales = context.Users
                .Where(u => u.ProductsSold.Count != 0 && u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .Include(x => x.ProductsSold)
                .ToList();

            
            var jsonExport = Mapper.Map<IEnumerable<User>, IEnumerable<UserWithSalesDto>>(usersWithSales);

            DefaultContractResolver contractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() };

            var jsonResult = JsonConvert.SerializeObject(jsonExport, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore

            });

            return jsonResult;

        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            List<ExportProductDto> productsInRange = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .ProjectTo<ExportProductDto>().ToList();

            string jsonExport = JsonConvert.SerializeObject(productsInRange);

            return jsonExport;
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var importedUsers = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(importedUsers);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var importedProducts = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(importedProducts);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categoriesFromJson = JsonConvert.DeserializeObject<Category[]>(inputJson);

            var validCategories = categoriesFromJson.Where(x => x.Name != null && x.Name.Length >= 3 && x.Name.Length <= 13).ToArray();

            context.Categories.AddRange(validCategories);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProductsFromJson = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            HashSet<int> categoryIds = context.Categories.Select(x => x.Id).ToHashSet();
            HashSet<int> productIds = context.Products.Select(x => x.Id).ToHashSet();

            var verifiedCategoryProducts = categoryProductsFromJson
                .Where(x => categoryIds.Contains(x.CategoryId) && productIds.Contains(x.ProductId)).ToList();


            context.CategoryProducts.AddRange(categoryProductsFromJson);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";

        }
    }
}