using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Models
{
    public class GameDto
    {
        public int Id { get; set; }
        public string UrlLink { get; set; }
        public DateTime Date { get; set; }
        public string Teams { get; set; }
        public string Division { get; set; }
        public string Position { get; set; }
    }
}
