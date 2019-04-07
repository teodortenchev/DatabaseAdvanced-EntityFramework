using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AutoMapper;
using ProductShop.Data;
using ProductShop.Models;
using System.Linq;
using System.Xml.Linq;
using ProductShop.Dtos.Import;
using ProductShop.Dtos.Export;
using AutoMapper.QueryableExtensions;
using System.Text;
using System.Xml;
using System;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (ProductShopContext context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                //DBInitializeFromXml(context);

                Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSales = context.Users
                .Where(u => u.ProductsSold.Count != 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new ExportUserSoldDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ExportProductSimpleDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToList()
                })
                .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportUserSoldDto>), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), usersWithSales, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Where(c => c.CategoryProducts.Count > 0)
                .Select(x => new ExportCategoryByCountDto
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportCategoryByCountDto>), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(x => new ExportUserWithProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new ExportProductCountDto
                    {
                        Count = x.ProductsSold.Count,
                        Products = x.ProductsSold.Select(p => new ExportProductSimpleDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).OrderByDescending(p => p.Price)
                        .ToList()
                    }
                })
                .Take(10)
                .ToList();

            var customExport = new ExportUsersDto
            {
                Count = context.Users.Where(u => u.ProductsSold.Count > 0).Count(),
                Users = users
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUsersDto), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            xmlSerializer.Serialize(new StringWriter(sb), customExport, namespaces);

            return sb.ToString().TrimEnd();
        }


        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price).Take(10)
                .Select(x => new ExportProductDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProductDto[]), new XmlRootAttribute("Products"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), productsInRange, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static void DBInitializeFromXml(ProductShopContext context)
        {
            ImportUsers(context, ImportFileHelper.UsersPath);
            ImportProducts(context, ImportFileHelper.ProductsPath);
            ImportCategories(context, ImportFileHelper.CategoriesPath);
            ImportCategoryProducts(context, ImportFileHelper.CategoriesProductsPath);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var userDtos = XmlDeserializer<ImportUserDto>(inputXml);

            var users = Mapper.Map<ImportUserDto[], User[]>(userDtos);

            context.Users.AddRange(users);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";

        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var productDtos = XmlDeserializer<ImportProductDto>(inputXml);

            var products = Mapper.Map<ImportProductDto[], Product[]>(productDtos);

            HashSet<int?> userIds = context.Users.Select(x => (int?)x.Id).ToHashSet();

            foreach (var product in products)
            {
                int? buyerId = product.BuyerId;

                if (!userIds.Contains(buyerId))
                {
                    product.BuyerId = null;
                }
            }


            context.Products.AddRange(products);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var categoryDtos = XmlDeserializer<ImportCategoryDto>(inputXml);

            var categories = Mapper.Map<ImportCategoryDto[], Category[]>(categoryDtos);

            context.Categories.AddRange(categories);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var categoryProductDtos = XmlDeserializer<ImportCategoryProductDto>(inputXml);

            var categoryProducts = Mapper.Map<ImportCategoryProductDto[], CategoryProduct[]>(categoryProductDtos);

            var verifiedCategoryProducts = new List<CategoryProduct>();

            HashSet<int> categoryIds = context.Categories.Select(x => x.Id).ToHashSet();
            HashSet<int> productIds = context.Categories.Select(x => x.Id).ToHashSet();

            foreach (var categoryProduct in categoryProducts)
            {
                int categoryId = categoryProduct.CategoryId;
                int productId = categoryProduct.ProductId;

                if (!categoryIds.Contains(categoryId) || !productIds.Contains(productId))
                {
                    continue;
                }

                verifiedCategoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(verifiedCategoryProducts);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }


        //This method will not work in Judge as the system will test with string and not an actual xml file.
        private static T[] XmlDeserializer<T>(string inputXml)
        {
            XDocument xmlDocument = XDocument.Load(inputXml);

            var rootName = xmlDocument.Root.Name.ToString();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootName));

            var input = File.ReadAllText(inputXml);

            var dtos = (T[])xmlSerializer.Deserialize(new StringReader(input));

            return dtos;
        }
    }
}