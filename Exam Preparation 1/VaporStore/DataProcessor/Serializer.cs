namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.ExportDtos;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var games = context.Genres.Where(g => genreNames.Contains(g.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games
                        .Where(g => g.Purchases.Count > 0)
                        .Select(gg => new
                        {
                            Id = gg.Id,
                            Title = gg.Name,
                            Developer = gg.Developer.Name,
                            Tags = String.Join(", ", gg.GameTags.Select(gt => gt.Tag.Name).ToArray()),
                            Players = gg.Purchases.Count
                        })
                    .OrderByDescending(gms => gms.Players).ThenBy(gms => gms.Id)
                    .ToArray(),

                    TotalPlayers = x.Games.Sum(s => s.Purchases.Count)
                })
                .OrderByDescending(d => d.TotalPlayers)
                .ThenBy(d => d.Id)
                .ToArray();


            var result = JsonConvert.SerializeObject(games, Newtonsoft.Json.Formatting.Indented);

            return result;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var purchaseType = Enum.Parse<PurchaseType>(storeType);

            var users = context.Users
                .Select(x => new ExportUserDto
                {
                    Username = x.Username,
                    Purchases = x.Cards
                        .SelectMany(c => c.Purchases)
                        .Where(p => p.Type == purchaseType)
                        .Select(pur => new PurchaseDto
                        {
                            Card = pur.Card.Number,
                            Cvc = pur.Card.Cvc,
                            Date = pur.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new GameDto
                            {
                                Title = pur.Game.Name,
                                Genre = pur.Game.Genre.Name,
                                Price = pur.Game.Price
                            }
                        })
                        .OrderBy(purchase => purchase.Date)
                        .ToArray(),

                    TotalSpent = x.Cards.SelectMany(p => p.Purchases).Where(p =>p.Type == purchaseType).Sum(p => p.Game.Price)
                })
                .Where(u => u.Purchases.Any())
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            string result = sb.ToString().TrimEnd();

            return result;
        }
    }
}