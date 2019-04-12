namespace Cinema.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies.Where(m => m.Rating >= rating)
                .ToArray()
                .Select(x => new
                {
                    MovieName = x.Title,
                    Rating = $"{x.Rating:F2}",
                    TotalIncomes = $"{x.Projections.SelectMany(b => b.Tickets).Sum(t => t.Price):F2}",
                    Customers = x.Projections.SelectMany(aa => aa.Tickets.Select(t => t.Customer))
                    .Select(cu => new
                    {
                        FirstName = cu.FirstName,
                        LastName = cu.LastName,
                        Balance = $"{cu.Balance:F2}"
                    })
                    .OrderByDescending(u => u.Balance)
                    .ThenBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToArray(),


                })
                .Where(mo => mo.Customers.Length > 0)
                .OrderByDescending(x => double.Parse(x.Rating))
                .ThenByDescending(x => decimal.Parse(x.TotalIncomes))
                .Take(10)
                .ToArray();

            var result = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return result;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var topCustomers = context.Customers.Where(c => c.Age >= age)
                .Select(x => new ExportTopCustomerDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SpentMoney = $"{x.Tickets.Sum(t => t.Price):F2}",
                    SpentTime = new TimeSpan(x.Tickets.Sum(u => u.Projection.Movie.Duration.Ticks)).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)
                })
                .OrderByDescending(x => decimal.Parse(x.SpentMoney))
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportTopCustomerDto[]), new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), topCustomers, namespaces);

            string result = sb.ToString().TrimEnd();

            return result;

        }
    }
}