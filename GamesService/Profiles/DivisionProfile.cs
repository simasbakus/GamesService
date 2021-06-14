using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Profiles
{
    public class DivisionProfile : Profile
    {
        public DivisionProfile()
        {
            this.CreateMap<Division, DivisionDto>();
        }
    }
}
