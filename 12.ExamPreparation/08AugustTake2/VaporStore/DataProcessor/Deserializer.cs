namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        private const string ImportedUsers = "Imported {0} with {1} cards";
        private const string ImportedGames = "Added {0} ({1}) with {2} tags";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            var jsonGames = JsonConvert.DeserializeObject<IEnumerable<ImportGamesDto>>(jsonString);

            foreach (var gameJson in jsonGames)
            {
                if (!IsValid(gameJson) || gameJson.Tags.Count() == 0)
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                var genre = context.Genres.FirstOrDefault(x => x.Name == gameJson.Genre)
                    ?? new Genre { Name = gameJson.Genre };
                var developer = context.Developers.FirstOrDefault(x => x.Name == gameJson.Developer)
                    ?? new Developer { Name = gameJson.Developer };

                var game = new Game
                {
                    Name = gameJson.Name,
                    Price = gameJson.Price,
                    Genre = genre,
                    Developer = developer,
                    ReleaseDate = gameJson.ReleaseDate.Value,
                };

                foreach (var tagJson in gameJson.Tags)
                {
                    var tag = context.Tags.FirstOrDefault(x => x.Name == tagJson)
                        ?? new Tag { Name = tagJson };

                    game.GameTags.Add(new GameTag { Tag = tag });
                }

                context.Games.Add(game);
                context.SaveChanges();
                output.AppendLine(string.Format(ImportedGames, gameJson.Name, gameJson.Genre, gameJson.Tags.Count()));
            }

            return output.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            var jsonUsers = JsonConvert.DeserializeObject<UserImportDto[]>(jsonString);

            foreach (var jsonUser in jsonUsers)
            {
                if (!IsValid(jsonUser))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                var user = new User
                {
                    FullName = jsonUser.FullName,
                    Username = jsonUser.Username,
                    Email = jsonUser.Email,
                    Age = jsonUser.Age,
                    Cards = jsonUser.Cards.Select(x => new Card
                    { 
                        Number = x.Number,
                        Cvc = x.CVC,
                        Type = x.Type
                    })
                    .ToArray()
                };

                context.Users.Add(user);
                context.SaveChanges();

                output.AppendLine(string.Format(ImportedUsers, jsonUser.Username, jsonUser.Cards.Length));
            }

            return output.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}