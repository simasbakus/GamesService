using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using GamesService.Repositories;
using GamesService.Services;
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
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _repository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository repository, IUserService userService, IMapper mapper)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("CurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Get currently authorized user id from claim
            string id = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;

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

        [HttpGet("Authenticate")]
        public IActionResult IsAuthenticated()
        {
            return Ok("User authenticated");
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserCred userCred)
        {
            /* AUTHENTICATES USER BY THE CREDENTIALS */

            string token = await _userService.Authenticate(userCred);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

        [HttpPost("CurrentUser/ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPasswords userPasswords)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;

            bool result = await _userService.ChangePassword(userPasswords, userId);

            if (!result)
                return StatusCode(500, "Something went wrong");

            return Ok("Password changed successfuly");
        }
    }
}
