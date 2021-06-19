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
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly IMongoCollection<RefreshToken> _refreshTokensCollection;

        public RefreshTokensRepository(IConfiguration configuration)
        {
            /* Connection to the Mongo Database */

            MongoClient client = new(configuration.GetConnectionString("MongoClient"));
            IMongoDatabase db = client.GetDatabase(configuration["MongoDB"]);
            _refreshTokensCollection = db.GetCollection<RefreshToken>(configuration["MongoRefreshTokensCollection"]);
        }

        public Task CreateRefreshToken(RefreshToken token)
        {
            return _refreshTokensCollection.InsertOneAsync(token);
        }

        public Task<RefreshToken> GetRefreshToken(string token)
        {
            return _refreshTokensCollection
                    .Find(Builders<RefreshToken>.Filter.Eq(rt => rt.Token, new BsonObjectId(new ObjectId(token))))
                    .FirstAsync();
        }

        public Task UseRefreshToken(RefreshToken token)
        {
            FilterDefinition<RefreshToken> refreshTokenFilter = Builders<RefreshToken>.Filter.Eq(rt => rt.Token, token.Token);
            UpdateDefinition<RefreshToken> refreshTokenUpdate = Builders<RefreshToken>.Update.Set(rt => rt.IsUsed, true);

            return _refreshTokensCollection.FindOneAndUpdateAsync(refreshTokenFilter, refreshTokenUpdate);
        }
    }
}
