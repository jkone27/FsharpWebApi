module HttpHandlers

open System
open Microsoft.Extensions.Logging
open Giraffe
open Services
open FSharp.Control.Tasks.V2
open UserInterface
open Microsoft.AspNetCore.Http

// https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md

// ---------------------------------
// Web app
// ---------------------------------

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model = { Text = greetings }
    let view = UI.index model
    htmlView view

let indexHandlerFeliz (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model = { Text = greetings }
    let view = FelizUi.index model
    htmlString (Feliz.ViewEngine.Render.htmlDocument view)

let loadSwaggerDefinition =
    text (OpenApiSpecProvider.GetSample().JsonValue.ToString())

let handleGetHelloWithName (name: string) =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
        let response = {|
            Text = sprintf "Hello, %s" name
        |}
        return! negotiate response next ctx
    }

let getPerson (id : int) : HttpHandler = 
    fun next ctx -> 
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let response = personsRepository.GetPersonById id
        match response with
        |Some(r) -> 
             return! json r next ctx
        |None ->
            return! RequestErrors.NOT_FOUND( $"person: {id} - not found" ) next ctx
    }

let getPeople (skipN,takeN) : HttpHandler = 
    fun next ctx -> 
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let response = personsRepository.GetPeople(skipN, takeN)
        match response with
        |x::xs -> 
             return! json response next ctx
        |[] ->
            return! RequestErrors.NOT_FOUND( $"no people found" ) next ctx
    }

let getPeopleByName name : HttpHandler = 
    fun next ctx -> 
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let response = personsRepository.GetPersonsByName name
        match response with
        |x::xs -> 
             return! json response next ctx
        |[] ->
            return! RequestErrors.NOT_FOUND( $"no people found" ) next ctx
    }

let storePerson : HttpHandler =
    fun next ctx ->
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let! personDto = ctx.BindModelAsync<_>()
        let result = personsRepository.InsertPerson personDto
        return! json result next ctx
    }

let updatePerson : HttpHandler =
    fun next ctx ->
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let! personDto = ctx.BindModelAsync<_>()
        let result = personsRepository.UpdatePerson personDto
        match result with
        |Some(r) -> 
            return! json r next ctx
        |None ->
            return! RequestErrors.NOT_FOUND( $"person: {id} - not found" ) next ctx
    }

let deletePerson (id : int) : HttpHandler =
    fun next ctx ->
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let result = personsRepository.DeletePerson id
        match result with
        |true -> 
            return! Successful.accepted( negotiate $"person: {id} deleted") next ctx
        |false ->
            return! RequestErrors.NOT_FOUND( $"person: {id} - not found" ) next ctx
    }

let webApp : HttpHandler =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler "world"
                routef "/hello/%s" indexHandlerFeliz
                routef "/api/hello/%s" handleGetHelloWithName
                route "/swagger/v1/swagger.json" >=> loadSwaggerDefinition
                routef "/api/persons/%i" getPerson
                routef "/api/persons?skip=%i&take=%i" getPeople
                route "/api/persons" >=> getPeople (0,100)
                routef "/api/persons/%s" getPeopleByName
            ]
        POST >=>
            choose [
                route "/api/persons" >=> storePerson
            ]
        PUT >=>
            choose [
                route "/api/persons" >=> updatePerson
            ]
        DELETE >=>
            choose [
                routef "/api/persons/%i" deletePerson
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message


