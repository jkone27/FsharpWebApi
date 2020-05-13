using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpWebApiSample.Contracts;
using CSharpWebApiSample.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api")]
    public class PersonsController : ControllerBase
    {

        private readonly ILogger<PersonsController> logger;
        private readonly PersonsRepository personsRepository;

        public PersonsController(ILogger<PersonsController> logger, PersonsRepository personsRepository)
        {
            this.logger = logger;
            this.personsRepository = personsRepository;
        }

        [HttpGet]
        [Route("persons/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await personsRepository.GetPersonByIdAsync(id);

            if(result is null)
            {
                return NotFound(id);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("persons")]
        public async Task<IActionResult> InsertPerson([FromBody] PersonDto person)
        {
            var result = await personsRepository.InsertPerson(person);

            return Ok(result);
        }

        [HttpPut]
        [Route("persons")]
        public async Task<IActionResult> UpdatePerson([FromBody] PersonDto person)
        {
            await personsRepository.UpdatePerson(person);

            return Ok(person);
        }

        [HttpDelete]
        [Route("persons/{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            await personsRepository.DeletePerson(id);

            return Ok(id);
        }
    }
}
