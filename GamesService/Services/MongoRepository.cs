using GamesService.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public class MongoRepository : IMongoRepository
    {
        private readonly IMongoCollection<Game> _gamesCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly FilterDefinition<Game> _emptyFilter = Builders<Game>.Filter.Empty;

        public MongoRepository(IConfiguration configuration)
        {
            /* Connection to the Mongo Database */

            MongoClient client = new MongoClient(configuration.GetConnectionString("MongoClient"));
            IMongoDatabase db = client.GetDatabase(configuration["MongoDB"]);
            _gamesCollection = db.GetCollection<Game>(configuration["MongoGamesCollection"]);
            _usersCollection = db.GetCollection<User>(configuration["MongoUsersCollection"]);
        }

        #region ------------- Users ---------------------
        public User GetUserByUsername(string username)
        {
            FilterDefinition<User> userFilter = Builders<User>.Filter.Eq(u => u.Username, username);

            return RunUsersQuery(userFilter);
        }

        public List<string> GetUserInfo(string id)
        {
            User user = GetUserById(id);

            List<string> userInfo = new List<string>()
            {
                user.Name,
                user.LName,
                user.NameInitial
            };

            return userInfo;
        }

        private User GetUserById(string id)
        {
            FilterDefinition<User> userFilter = Builders<User>.Filter.Eq(u => u.Id, new BsonObjectId(new ObjectId(id)));

            return RunUsersQuery(userFilter);
        }

        private User RunUsersQuery(FilterDefinition<User> userFilter)
        {
            return _usersCollection
                    .Find(userFilter)
                    .First();
        }
        #endregion --------------------------------------


        #region ------------- Games ---------------------
        public Task<List<Game>> GetAllGamesAsync(string userId, List<string> divisions)
        {
            User user = GetUserById(userId);

            FilterDefinition<Game> refFilter = SetRefFilter(user);
            FilterDefinition<Game> divisionFilter = CreateDivisionsFilter(divisions);

            return RunGamesQueryAsync(refFilter, _emptyFilter, divisionFilter);
        }

        public Task<List<Game>> GetGamesByMonthAsync(string userId, string dateStr, List<string> divisions)
        {
            User user = GetUserById(userId);

            FilterDefinition<Game> refFilter = SetRefFilter(user);

            DateTime date = DateTime.ParseExact(dateStr, "yyyy-MM", null);
            FilterDefinition<Game> monthFilter = Builders<Game>.Filter.Gte(g => g.Date, date)
                                                 & Builders<Game>.Filter.Lt(g => g.Date, date.AddMonths(1));

            FilterDefinition<Game> divisionFilter = CreateDivisionsFilter(divisions);

            return RunGamesQueryAsync(refFilter, monthFilter, divisionFilter);
        }

        private Task<List<Game>> RunGamesQueryAsync(FilterDefinition<Game> refFilter, FilterDefinition<Game> monthFilter, FilterDefinition<Game> divisionFilter)
        {
            return _gamesCollection
                    .Find(refFilter & monthFilter & divisionFilter)
                    .Sort("{ Date: -1 }")
                    .ToListAsync();
        }
        #endregion -------------------------------------


        #region ---------------- Filter Builders ---------------------
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

        #endregion --------------------------------------------------
    }
}
