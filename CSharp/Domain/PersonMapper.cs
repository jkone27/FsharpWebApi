using AutoMapper;
using CSharpWebApiSample.Contracts;

namespace CSharpWebApiSample.Domain
{
    public class PersonMapper : Profile
    {
        public PersonMapper()
        {
            CreateMap<Person, PersonDto>();

            CreateMap<PersonDto, Person>();
        }
    }
}
