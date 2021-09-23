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
            this.CreateMap<Game, GameDtoPublic>()
                .ForMember(dest => dest.UrlLink, opt => opt.MapFrom(src => $"http://m.hockey.lt/#/rezultatai/rungtynes/{src.Id}"));

            this.CreateMap<Game, GameDto>()
                    .ForMember(dest => dest.UrlLink, opt => opt.MapFrom(src => $"http://m.hockey.lt/#/rezultatai/rungtynes/{src.Id}"))
                    .ForMember(dest => dest.Position, opt => opt.MapFrom((src, dst, _, context) => GetPosition(src, context)));
        }

        private static string GetPosition(Game src, ResolutionContext context)
        {
            string LNamePattern = context.Options.Items["LNamePattern"].ToString();
            string Name = context.Options.Items["Name"].ToString();
            string NameInit = context.Options.Items["NameInit"].ToString();

            bool isHeadReferee = (Regex.IsMatch(src.HeadRef1, LNamePattern, RegexOptions.IgnoreCase)
                                  && (Regex.IsMatch(src.HeadRef1, Name, RegexOptions.IgnoreCase)
                                      || Regex.IsMatch(src.HeadRef1, NameInit, RegexOptions.IgnoreCase)))
                              || (Regex.IsMatch(src.HeadRef2, LNamePattern, RegexOptions.IgnoreCase)
                                  && (Regex.IsMatch(src.HeadRef2, Name, RegexOptions.IgnoreCase)
                                      || Regex.IsMatch(src.HeadRef2, NameInit, RegexOptions.IgnoreCase)));

            return isHeadReferee ? "Referee" : "Linesman";
        }
    }
}
