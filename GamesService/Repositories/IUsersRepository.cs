using GamesService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(string id);
        Task ChangePassword(User user, string newPassword);
    }
}
