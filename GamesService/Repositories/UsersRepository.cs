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
    public class UsersRepository : IUsersRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersRepository(IConfiguration configuration)
        {
            /* Connection to the Mongo Database */

            MongoClient client = new(configuration.GetConnectionString("MongoClient"));
            IMongoDatabase db = client.GetDatabase(configuration["MongoDB"]);
            _usersCollection = db.GetCollection<User>(configuration["MongoUsersCollection"]);
        }

        public Task ChangePassword(User user, string newPassword)
        {
            FilterDefinition<User> userFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            UpdateDefinition<User> userUpdate = Builders<User>.Update.Set(u => u.Password, newPassword);

            return _usersCollection.FindOneAndUpdateAsync(userFilter, userUpdate);
        }

        public Task<User> GetUserById(string id)
        {
            FilterDefinition<User> userFilter = Builders<User>.Filter.Eq(u => u.Id, new BsonObjectId(new ObjectId(id)));

            return RunUsersQueryAsync(userFilter);
        }

        public Task<User> GetUserByUsername(string username)
        {
            FilterDefinition<User> userFilter = Builders<User>.Filter.Eq(u => u.Username, username);

            return RunUsersQueryAsync(userFilter);
        }

        private Task<User> RunUsersQueryAsync(FilterDefinition<User> userFilter)
        {
            return _usersCollection
                    .Find(userFilter)
                    .FirstAsync();
        }
    }
}
