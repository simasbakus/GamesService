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
    public class UsersController : ControllerBase
    {
        private readonly IMongoRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IMongoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            // Check if requested user id matches the user id in the token
            if (id != User.Claims.FirstOrDefault(c => c.Type == "Id").Value)
                return Unauthorized();

            try
            {
                User user = await _repository.GetUserById(id);

                UserDto userDto = _mapper.Map<UserDto>(user);

                return Ok(userDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }
    }
}
