using AutoMapper;
using CSharpWebApiSample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
