using GamesService.Models;

namespace GamesService.Services
{
    public interface IUserAuthentication
    {
        string Authenticate(UserCred userCred);
    }
}
