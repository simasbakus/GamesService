using GamesService.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Repositories
{
    public class DivisionsRepository : IDivisionsRepository
    {
        private readonly IMongoCollection<Division> _divisionsCollection;

        public DivisionsRepository(IConfiguration configuration)
        {
            /* Connection to the Mongo Database */

            MongoClient client = new(configuration.GetConnectionString("MongoClient"));
            IMongoDatabase db = client.GetDatabase(configuration["MongoDB"]);
            _divisionsCollection = db.GetCollection<Division>(configuration["MongoDivisionsCollection"]);
        }

        public Task<List<Division>> GetDivisions()
        {
            return _divisionsCollection
                    .Find(Builders<Division>.Filter.Empty)
                    .ToListAsync();
        }
    }
}
