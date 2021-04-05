using GamesService.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public interface IMongoRepository
    {
        Task<List<Game>> GetAllGamesAsync(string userId, List<string> divisions);
        Task<List<Game>> GetGamesByMonthAsync(string userId, string dateStr, List<string> divisions);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(string id);
        Task<List<string>> GetUserInfo(string id);
    }
}
