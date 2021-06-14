using AutoMapper;
using GamesService.Entities;
using GamesService.Models;
using GamesService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    public class DivisionsController : ControllerBase
    {
        private readonly IDivisionsRepository _repository;
        private readonly IMapper _mapper;

        public DivisionsController(IDivisionsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Division> result = await _repository.GetDivisions();

                List<DivisionDto> divisions = _mapper.Map<List<DivisionDto>>(result);

                return Ok(divisions);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }
    }
}
