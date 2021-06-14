using GamesService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Repositories
{
    public interface IDivisionsRepository
    {
        Task<List<Division>> GetDivisions();
    }
}
