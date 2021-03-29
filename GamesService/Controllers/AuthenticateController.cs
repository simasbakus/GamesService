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
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserAuthentication _authentication;

        public AuthenticateController(IUserAuthentication authentication)
        {
            _authentication = authentication;
        }

        [Authorize]
        [HttpGet]
        public IActionResult IsAuthenticated()
        {
            return Ok("User authenticated");
        }

        [HttpGet("Test")]
        public IActionResult PingTest()
        {
            return Ok("Ping Test Successful!");
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCred userCred)
        {
            /* AUTHENTICATES USER BY THE CREDENTIALS */

            string token = _authentication.Authenticate(userCred);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }
    }
}
