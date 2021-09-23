using GamesService.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Repositories
{
    public class GamesRepository : IGamesRepository
    {
        private readonly string _gamesCollectionBasePart;
        private readonly IMongoDatabase _db;
        private readonly FilterDefinition<Game> _emptyFilter = Builders<Game>.Filter.Empty;

        private const int NEW_SEASON_START_MONTH = 8;
        private const string DATE_FORMAT = "yyyy-MM";

        public GamesRepository(IConfiguration configuration)
        {
            /* Connection to the Mongo Database */

            MongoClient client = new(configuration.GetConnectionString("MongoClient"));
            _db = client.GetDatabase(configuration["MongoDB"]);
            _gamesCollectionBasePart = configuration["MongoGamesCollection"];
        }

        public async Task<List<Game>> GetAllGamesAsync(User user, List<string> divisions)
        {
            return await RunGamesQueryAsync(user, string.Empty, divisions);
        }

        public async Task<List<Game>> GetGamesByMonthAsync(User user, string dateStr, List<string> divisions)
        {
            return await RunGamesQueryAsync(user, dateStr, divisions);
        }

        private Task<List<Game>> RunGamesQueryAsync(User user, string date, List<string> divisions)
        {
            string gamesCollectionSeasonDates = GetSeasonDatesForCollectionName(date);

            FilterDefinition<Game> refFilter = user != null ? SetRefFilter(user) : _emptyFilter;
            FilterDefinition<Game> monthFilter = _emptyFilter;
            FilterDefinition<Game> divisionFilter = CreateDivisionsFilter(divisions);

            if (!string.IsNullOrEmpty(date))
            {
                DateTime dateParsed = DateTime.ParseExact(date, DATE_FORMAT, null);
                monthFilter = Builders<Game>.Filter.Gte(g => g.Date, dateParsed)
                    & Builders<Game>.Filter.Lt(g => g.Date, dateParsed.AddMonths(1));
            }

            return _db.GetCollection<Game>(_gamesCollectionBasePart + gamesCollectionSeasonDates)
                    .Find(refFilter & monthFilter & divisionFilter)
                    .Sort("{ Date: -1 }")
                    .ToListAsync();
        }

        private static FilterDefinition<Game> SetRefFilter(User user)
        {
            return Builders<Game>.Filter.Regex("HeadRef1", new BsonRegularExpression($"/{user.LNamePattern}/i")) & (Builders<Game>.Filter.Regex("HeadRef1", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("HeadRef1", new BsonRegularExpression(user.NameInitial)))
                   | Builders<Game>.Filter.Regex("HeadRef2", new BsonRegularExpression($"/{user.LNamePattern}/i")) & (Builders<Game>.Filter.Regex("HeadRef2", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("HeadRef2", new BsonRegularExpression(user.NameInitial)))
                   | Builders<Game>.Filter.Regex("Linesman1", new BsonRegularExpression($"/{user.LNamePattern}/i")) & (Builders<Game>.Filter.Regex("Linesman1", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("Linesman1", new BsonRegularExpression(user.NameInitial)))
                   | Builders<Game>.Filter.Regex("Linesman2", new BsonRegularExpression($"/{user.LNamePattern}/i")) & (Builders<Game>.Filter.Regex("Linesman2", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("Linesman2", new BsonRegularExpression(user.NameInitial)));
        }

        private FilterDefinition<Game> CreateDivisionsFilter(List<string> divisions)
        {
            FilterDefinition<Game> divisionFilter = _emptyFilter;

            if (divisions.Count > 0)
            {
                foreach (string division in divisions)
                {
                    if (divisionFilter == _emptyFilter)
                        divisionFilter = Builders<Game>.Filter.Eq(g => g.Division, division);
                    else
                        divisionFilter |= Builders<Game>.Filter.Eq(g => g.Division, division);
                }
            }
            return divisionFilter;
        }

        private static string GetSeasonDatesForCollectionName(string date)
        {
            DateTime dateParsed = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                dateParsed = DateTime.ParseExact(date, DATE_FORMAT, null);
            }

            return dateParsed.Month < NEW_SEASON_START_MONTH ? $"{dateParsed.Year - 1}-{dateParsed.Year}" 
                : $"{dateParsed.Year}-{dateParsed.Year + 1}";
        }
    }
}
