using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Teams { get; set; }
        public string Division { get; set; }
        public string HeadRef1 { get; set; }
        public string HeadRef2 { get; set; }
        public string Linesman1 { get; set; }
        public string Linesman2 { get; set; }
    }
}
