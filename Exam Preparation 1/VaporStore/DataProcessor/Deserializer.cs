namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.ImportDtos;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gameDtos = JsonConvert.DeserializeObject<ImportGamesDto[]>(jsonString);

            List<Game> games = new List<Game>();

            var sb = new StringBuilder();

            foreach (var gameDto in gameDtos)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Game game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = GetDeveloper(context, gameDto.Developer),
                    Genre = GetGenre(context, gameDto.Genre),

                };

                foreach (var tag in gameDto.Tags)
                {
                    game.GameTags.Add(new GameTag
                    {
                        Game = game,
                        Tag = GetTag(context, tag),
                    });
                }

                games.Add(game);

                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");

            }

            context.Games.AddRange(games);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            List<User> users = new List<User>();

            var sb = new StringBuilder();

            foreach (var userDto in userDtos)
            {
                bool validCardType = true;

                if (!IsValid(userDto) || userDto.Cards.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                User user = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };



                foreach (var card in userDto.Cards)
                {
                    if (!IsValid(card) || userDto.Cards.Any(x => x.Type != "Debit" && x.Type != "Credit"))
                    {
                        sb.AppendLine("Invalid Data");
                        validCardType = false;
                        break;
                    }
                }

                if (validCardType == false)
                {
                    continue;
                }

                foreach (var card in userDto.Cards)
                {
                    user.Cards.Add(GetCard(context, card));
                }

                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");

                users.Add(user);

            }
            context.Users.AddRange(users);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        private static Card GetCard(VaporStoreDbContext context, ImportCardDto currentCard)
        {
            Card card = context.Cards.FirstOrDefault(x => x.Number == currentCard.Number);

            if (card == null)
            {
                card = new Card
                {
                    Number = currentCard.Number,
                    Cvc = currentCard.Cvc,
                    Type = Enum.Parse<CardType>(currentCard.Type)
                };
            }

            return card;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));

            var purchaseDtos = (ImportPurchaseDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            List<Purchase> purchases = new List<Purchase>();

            var sb = new StringBuilder();

            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto) && purchaseDto.Type != "Retail" && purchaseDto.Type != "Digital")
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var game = GetGame(context, purchaseDto.Title);

                var card = CheckCard(context, purchaseDto.Card);

                if (game == null || card == null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var user = context.Users.FirstOrDefault(x => x.Cards.Any(c => c.Number == card.Number));

                Purchase purchase = new Purchase
                {
                    Type = Enum.Parse<PurchaseType>(purchaseDto.Type),
                    ProductKey = purchaseDto.Key,
                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Card = card,
                    Game = game,
                };

                purchases.Add(purchase);

                sb.AppendLine($"Imported {game.Name} for {user.Username}");

            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        private static Card CheckCard(VaporStoreDbContext context, string cardNumber)
        {
            Card card = context.Cards.FirstOrDefault(x => x.Number == cardNumber);

            if (card == null)
            {
                return null;
            }

            return card;
        }

        private static Game GetGame(VaporStoreDbContext context, string title)
        {
            Game game = context.Games.FirstOrDefault(x => x.Name == title);

            if (game == null)
            {
                return null;
            }

            return game;
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }

        private static Tag GetTag(VaporStoreDbContext context, string tagName)
        {
            var tag = context.Tags.FirstOrDefault(x => x.Name == tagName);

            if (tag == null)
            {
                tag = new Tag
                {
                    Name = tagName
                };

                context.Tags.Add(tag);
                context.SaveChanges();
            }

            return tag;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string name)
        {
            Developer developer = context.Developers.FirstOrDefault(x => x.Name == name);

            if (developer == null)
            {
                developer = new Developer
                {
                    Name = name
                };

                context.Developers.Add(developer);
                context.SaveChanges();
            }

            return developer;
        }

        private static Genre GetGenre(VaporStoreDbContext context, string name)
        {
            Genre genre = context.Genres.FirstOrDefault(x => x.Name == name);

            if (genre == null)
            {
                genre = new Genre
                {
                    Name = name
                };

                context.Genres.Add(genre);
                context.SaveChanges();
            }

            return genre;
        }
    }
}