using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                                                                             (Regex.IsMatch(src.HeadRef1, context.Options.Items["LNamePattern"].ToString()) 
                                                                              && (Regex.IsMatch(src.HeadRef1, context.Options.Items["Name"].ToString())
                                                                                  || Regex.IsMatch(src.HeadRef1, context.Options.Items["NameInit"].ToString())))
                                                                         || (Regex.IsMatch(src.HeadRef2, context.Options.Items["LNamePattern"].ToString())
                                                                             && (Regex.IsMatch(src.HeadRef2, context.Options.Items["Name"].ToString())
                                                                                 || Regex.IsMatch(src.HeadRef2, context.Options.Items["NameInit"].ToString())))
                                                                         ? "Referee" : "Linesman"));
        }
    }
}
