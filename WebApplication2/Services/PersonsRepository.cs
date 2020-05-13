using AutoMapper;
using CSharpWebApiSample.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpWebApiSample.Domain
{
    public class PersonsRepository
    {
        private readonly PersonsContext context;
        private readonly IMapper mapper;

        public PersonsRepository(PersonsContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == id);

            return mapper.Map<PersonDto>(person);
        }

        public async Task<List<PersonDto>> GetPersonsByName(string name)
        {
            var persons = await context.Persons.Where(p => p.Name == name).ToListAsync();

            return mapper.Map<List<PersonDto>>(persons);
        }

        public async Task<PersonDto> InsertPerson(PersonDto person)
        {
            var domainPerson = mapper.Map<Person>(person);

            var persons = await context.Persons.AddAsync(domainPerson);

            await context.SaveChangesAsync();

            return mapper.Map<PersonDto>(persons.Entity);
        }

        public async Task UpdatePerson(PersonDto person)
        {
            var domainPerson = await context.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);

            if (domainPerson != null)
            {
                domainPerson.Age = person.Age;
                domainPerson.Name = person.Name;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeletePerson(int id)
        {
            var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == id);

            if(person != null)
            {
                context.Persons.Remove(person);

                await context.SaveChangesAsync();
            }
        }
    }
}
