namespace Services
open Microsoft.Extensions.Options

type PersonsRepository(options: AppSettings) =
    
    let ctx = ctxFactory options.DbConfiguration.ConnectionString
    //https://fsprojects.github.io/SQLProvider/core/querying.html
    //https://fsprojects.github.io/SQLProvider/core/async.html

    let getDbPerson id = 
        query {
             for person in ctx.Dbo.Persons do
             where (person.Id = id)
             select (person)
         } |> Seq.tryHead

    member _.GetPersonById(id) =
        getDbPerson id
        |> Option.map PersonDto.Map

    member _.GetPersonsByName(name) =
        query {
            for person in ctx.Dbo.Persons do
            where (person.Name = name)
            select (person)
        } 
        |> Seq.map PersonDto.Map
        |> Seq.toList

    member _.InsertPerson(personDto) =
       
       let dbPerson = ctx.Dbo.Persons.Create()
       dbPerson.Age <- personDto.Age
       dbPerson.Name <- personDto.Name
       
       ctx.SubmitUpdates();

    member _.DeletePerson(id) =

       getDbPerson id 
       |> Option.iter (fun dbPerson -> 
           dbPerson.Delete())

       ctx.SubmitUpdates();

    member _.UpdatePerson(personDto) =
   
       getDbPerson personDto.Id 
       |> Option.iter (fun dbPerson ->
           dbPerson.Age <- personDto.Age
           dbPerson.Name <- personDto.Name
       )
   
       ctx.SubmitUpdates();
        

