using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using GamesService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IMongoRepository _repository;
        private readonly IMapper _mapper;

        public GamesController(IMongoRepository repository, IMapper mapper)
        {
            _repository = repository;
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
                List<Game> results = await _repository.GetAllGamesAsync(userId, ConvertStrToList(Divisions));

                List<string> userInfo = _repository.GetUserInfo(userId);

                List<GameDto> games = _mapper.Map<List<GameDto>>(results, opt =>
                {
                    opt.Items["Name"] = userInfo[0];
                    opt.Items["Lname"] = userInfo[1];
                    opt.Items["NameInit"] = userInfo[2];
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
                List<Game> results = await _repository.GetGamesByMonthAsync(userId, date, ConvertStrToList(Divisions));

                List<string> userInfo = _repository.GetUserInfo(userId);

                List<GameDto> games = _mapper.Map<List<GameDto>>(results, opt =>
                {
                    opt.Items["Name"] = userInfo[0];
                    opt.Items["Lname"] = userInfo[1];
                    opt.Items["NameInit"] = userInfo[2];
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
