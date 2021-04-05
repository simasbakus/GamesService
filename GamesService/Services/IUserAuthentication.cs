using GamesService.Models;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public interface IUserAuthentication
    {
        Task<string> Authenticate(UserCred userCred);
    }
}
