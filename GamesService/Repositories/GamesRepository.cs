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
        private readonly IMongoCollection<Game> _gamesCollection;
        private readonly FilterDefinition<Game> _emptyFilter = Builders<Game>.Filter.Empty;

        public GamesRepository(IConfiguration configuration)
        {
            /* Connection to the Mongo Database */

            MongoClient client = new(configuration.GetConnectionString("MongoClient"));
            IMongoDatabase db = client.GetDatabase(configuration["MongoDB"]);
            _gamesCollection = db.GetCollection<Game>(configuration["MongoGamesCollection"]);
        }

        public async Task<List<Game>> GetAllGamesAsync(User user, List<string> divisions)
        {
            FilterDefinition<Game> refFilter = SetRefFilter(user);
            FilterDefinition<Game> divisionFilter = CreateDivisionsFilter(divisions);

            return await RunGamesQueryAsync(refFilter, _emptyFilter, divisionFilter);
        }

        public async Task<List<Game>> GetGamesByMonthAsync(User user, string dateStr, List<string> divisions)
        {
            FilterDefinition<Game> refFilter = SetRefFilter(user);

            DateTime date = DateTime.ParseExact(dateStr, "yyyy-MM", null);
            FilterDefinition<Game> monthFilter = Builders<Game>.Filter.Gte(g => g.Date, date)
                                                 & Builders<Game>.Filter.Lt(g => g.Date, date.AddMonths(1));

            FilterDefinition<Game> divisionFilter = CreateDivisionsFilter(divisions);

            return await RunGamesQueryAsync(refFilter, monthFilter, divisionFilter);
        }

        private Task<List<Game>> RunGamesQueryAsync(FilterDefinition<Game> refFilter, FilterDefinition<Game> monthFilter, FilterDefinition<Game> divisionFilter)
        {
            return _gamesCollection
                    .Find(refFilter & monthFilter & divisionFilter)
                    .Sort("{ Date: -1 }")
                    .ToListAsync();
        }

        private static FilterDefinition<Game> SetRefFilter(User user)
        {
            return Builders<Game>.Filter.Regex("HeadRef1", new BsonRegularExpression(user.LName)) & (Builders<Game>.Filter.Regex("HeadRef1", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("HeadRef1", new BsonRegularExpression(user.NameInitial)))
                   | Builders<Game>.Filter.Regex("HeadRef2", new BsonRegularExpression(user.LName)) & (Builders<Game>.Filter.Regex("HeadRef2", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("HeadRef2", new BsonRegularExpression(user.NameInitial)))
                   | Builders<Game>.Filter.Regex("Linesman1", new BsonRegularExpression(user.LName)) & (Builders<Game>.Filter.Regex("Linesman1", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("Linesman1", new BsonRegularExpression(user.NameInitial)))
                   | Builders<Game>.Filter.Regex("Linesman2", new BsonRegularExpression(user.LName)) & (Builders<Game>.Filter.Regex("Linesman2", new BsonRegularExpression(user.Name)) | Builders<Game>.Filter.Regex("Linesman2", new BsonRegularExpression(user.NameInitial)));
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
    }
}
