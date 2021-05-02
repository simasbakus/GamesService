using GamesService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Repositories
{
    public interface IGamesRepository
    {
        Task<List<Game>> GetAllGamesAsync(User user, List<string> divisions);
        Task<List<Game>> GetGamesByMonthAsync(User user, string dateStr, List<string> divisions);
    }
}
