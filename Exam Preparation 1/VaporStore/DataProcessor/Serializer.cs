namespace VaporStore.DataProcessor
{
	using System;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var games = context.Games.Where(x => genreNames.Contains(x.Genre.Name) && x.Purchases.Any())
                .Select(u => new
                 {
                     Id = u.Id,
                     Genre = u.Genre.Name,
                     Games = u.Genre.Games.Select(g => new
                     {
                         Id = g.Id,
                         Title = g.Name,
                         Developer = g.Developer.Name,
                         Tags = String.Join(", ", g.GameTags.Select(x => x.Tag.Name).ToArray()),
                         Players = g.Purchases.Count()
                     })
                    .OrderByDescending(game => game.Players)
                    .ThenBy(game => game.Id)
                    .ToArray(),

                     TotalPlayers = context.Games.Sum(x => x.Purchases.Count)
                 }).ToArray();

            var result = JsonConvert.SerializeObject(games, Formatting.Indented);

            return result;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			throw new NotImplementedException();
		}
	}
}