using GamesService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Repositories
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> GetRefreshToken(string token);
        Task UseRefreshToken(RefreshToken token);
        Task CreateRefreshToken(RefreshToken token);
    }
}
