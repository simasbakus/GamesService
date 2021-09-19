using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using GamesService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IGamesRepository _gamesRepository;
        private readonly IMapper _mapper;

        public GamesController(IUsersRepository usersRepository, IGamesRepository gamesRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _gamesRepository = gamesRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("Public")]
        public async Task<IActionResult> GetGamesPublic(string Divisions = "")
        {
            /* GETS ALL GAMES FOR THE PUBLIC, OPTIONAL DIVISIONS FOR FILTERING */
            try
            {
                List<Game> results = await _gamesRepository.GetAllGamesAsync(null, ConvertStrToList(Divisions));

                List<GameDtoPublic> games = _mapper.Map<List<GameDtoPublic>>(results);

                return Ok(games);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGames(string Divisions = "")
        {
            /* GETS ALL GAMES FOR THE AUTHENTICATED USER, OPTIONAL DIVISIONS FOR FILTERING */
             
            //Gets user Id from token claim
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

            try
            {
                User user = await _usersRepository.GetUserById(userId);

                List<Game> results = await _gamesRepository.GetAllGamesAsync(user, ConvertStrToList(Divisions));

                List<GameDto> games = _mapper.Map<List<GameDto>>(results, opt =>
                {
                    opt.Items["Name"] = user.Name;
                    opt.Items["LNamePattern"] = user.LNamePattern;
                    opt.Items["NameInit"] = user.NameInitial;
                });

                return Ok(games);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }

        }

        [AllowAnonymous]
        [HttpGet("Public/{date}")]
        public async Task<IActionResult> GetMonthGamesPublic(string date, string Divisions = "")
        {
            /* GETS SPECIFIC MONTH GAMES FOR THE PUBLIC, OPTIONAL DIVISIONS FOR FILTERING */

            try
            {
                List<Game> results = await _gamesRepository.GetGamesByMonthAsync(null, date, ConvertStrToList(Divisions));

                List<GameDtoPublic> games = _mapper.Map<List<GameDtoPublic>>(results);

                return Ok(games);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetMonthGames(string date, string Divisions = "")
        {
            /* GETS SPECIFIC MONTH GAMES FOR THE AUTHENTICATED USER, OPTIONAL DIVISIONS FOR FILTERING */

            //Gets user Id from token claim
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

            try
            {
                User user = await _usersRepository.GetUserById(userId);

                List<Game> results = await _gamesRepository.GetGamesByMonthAsync(user, date, ConvertStrToList(Divisions));

                List<GameDto> games = _mapper.Map<List<GameDto>>(results, opt =>
                {
                    opt.Items["Name"] = user.Name;
                    opt.Items["LNamePattern"] = user.LNamePattern;
                    opt.Items["NameInit"] = user.NameInitial;
                });

                return Ok(games);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }

        private static List<string> ConvertStrToList(string divisions)
        {
            return divisions != "" ? divisions.Split(',').ToList() : new List<string>();
        }
    }
}
