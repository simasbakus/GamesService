using GamesService.Entities;
using GamesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Authenticate(UserCred userCred);
        Task<AuthenticationResponse> RefreshTokens(string token, string refreshToken);
        Task<bool> ChangePassword(UserPasswords userPasswords, string userId);
    }
}
