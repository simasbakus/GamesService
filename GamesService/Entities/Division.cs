using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Entities
{
    public class Division
    {
        public BsonObjectId Id { get; set; }
        public string Label { get; set; }
        public string QueryString { get; set; }
    }
}
