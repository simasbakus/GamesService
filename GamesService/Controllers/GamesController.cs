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

        [HttpGet]
        public async Task<IActionResult> Get(string Divisions = "")
        {
            /* GETS ALL GAMES FOR THE AUTHENTICATED USER, OPTIONAL DIVISIONS FOR FILTERING */
             
            //Gets user Id from token claim
            string userId = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;

            try
            {
                User user = await _usersRepository.GetUserById(userId);

                List<Game> results = await _gamesRepository.GetAllGamesAsync(user, ConvertStrToList(Divisions));

                List<GameDto> games = _mapper.Map<List<GameDto>>(results, opt =>
                {
                    opt.Items["Name"] = user.Name;
                    opt.Items["Lname"] = user.LName;
                    opt.Items["NameInit"] = user.NameInitial;
                });

                return Ok(games);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }

        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetMonth(string date, string Divisions = "")
        {
            /* GETS SPECIFIC MONTH GAMES FOR THE AUTHENTICATED USER, OPTIONAL DIVISIONS FOR FILTERING */

            //Gets user Id from token claim
            string userId = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;

            try
            {
                User user = await _usersRepository.GetUserById(userId);

                List<Game> results = await _gamesRepository.GetGamesByMonthAsync(user, date, ConvertStrToList(Divisions));

                List<GameDto> games = _mapper.Map<List<GameDto>>(results, opt =>
                {
                    opt.Items["Name"] = user.Name;
                    opt.Items["Lname"] = user.LName;
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
