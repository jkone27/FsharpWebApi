namespace Services
open Microsoft.Extensions.Options

//https://github.com/fsprojects/SQLProvider/blob/master/tests/SqlProvider.Tests/Readme.md

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

    member _.GetPeople (?skipN, ?takeN) =
        let s = defaultArg skipN 0
        let t = defaultArg takeN 100
        query {
            for person in ctx.Dbo.Persons do
            sortBy person.Id
            select (person)
            skip s
            take t
        } 
        |> Seq.map PersonDto.Map
        |> Seq.toList

    member _.GetPersonsByName(name) =
        query {
            for person in ctx.Dbo.Persons do
            where (person.Name = name)
            select (person)
        } 
        |> Seq.map PersonDto.Map
        |> Seq.toList

    member _.InsertPerson(personDto) =
       
       let mutable dbPerson = ctx.Dbo.Persons.Create()
       dbPerson.Age <- personDto.Age
       dbPerson.Name <- personDto.Name
       
       ctx.SubmitUpdates()

       dbPerson |> PersonDto.Map

    member _.DeletePerson(id) =

       let dbPersonOption = getDbPerson id 
       
       dbPersonOption
       |> Option.iter (fun dbPerson -> 
           dbPerson.Delete())

       ctx.SubmitUpdates()

       dbPersonOption.IsSome

    member _.UpdatePerson(personDto) =
   
       let mutable dbPersonOption = getDbPerson personDto.Id 
       
       dbPersonOption
       |> Option.iter (fun dbPerson ->
           dbPerson.Age <- personDto.Age
           dbPerson.Name <- personDto.Name
       )
   
       ctx.SubmitUpdates()

       dbPersonOption 
       |> Option.map PersonDto.Map
        

