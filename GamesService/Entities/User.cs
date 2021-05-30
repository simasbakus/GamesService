using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Entities
{
    public class User
    {
        public BsonObjectId Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LName { get; set; }
        public string LNamePattern { get; set; }
        public string NameInitial { get; set; }
        public string Role { get; set; }
    }
}
