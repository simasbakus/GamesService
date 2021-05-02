using GamesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public interface ICryptographyService
    {
        string DecryptCredential(string credential, string IV);
        (string, string) EncryptCredential(string credential);
    }
}
