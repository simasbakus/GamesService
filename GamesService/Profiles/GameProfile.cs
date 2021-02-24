using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Profiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            this.CreateMap<Game, GameDto>()
                    .ForMember(dest => dest.UrlLink, opt => opt.MapFrom(src => "http://m.hockey.lt/#/rezultatai/rungtynes/" + src.Id.ToString()))
                    .ForMember(dest => dest.Position, opt => opt.MapFrom((src, dst, _, context) => 
                                                                             (src.HeadRef1.Contains(context.Options.Items["Lname"].ToString()) 
                                                                              && (src.HeadRef1.Contains(context.Options.Items["Name"].ToString())
                                                                                  || src.HeadRef1.Contains(context.Options.Items["NameInit"].ToString())))
                                                                         || (src.HeadRef2.Contains(context.Options.Items["Lname"].ToString()) 
                                                                             && (src.HeadRef2.Contains(context.Options.Items["Name"].ToString())
                                                                                 || src.HeadRef2.Contains(context.Options.Items["NameInit"].ToString())))
                                                                         ? "Referee" : "Linesman"));
        }
    }
}
