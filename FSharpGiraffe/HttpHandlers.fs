module HttpHandlers

open System
open Microsoft.Extensions.Logging
open Saturn.Endpoint
open Services
open UserInterface
open Microsoft.AspNetCore.Http
open Giraffe

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

let getPeople : HttpHandler = 
    fun next ctx -> 
    task {
        // Parse query params manually or use ctx.TryBindQueryString
        let skipN = ctx.TryGetQueryStringValue "skip" |> Option.map int |> Option.defaultValue 0
        let takeN = ctx.TryGetQueryStringValue "take" |> Option.map int |> Option.defaultValue 100

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

let webApp : Giraffe.EndpointRouting.Routers.Endpoint list =
    router {
        // GET endpoints
        get "/" (indexHandler "world")
        getf "/hello/%s" indexHandlerFeliz
        getf "/api/hello/%s" handleGetHelloWithName
        get "/swagger/v1/swagger.json" loadSwaggerDefinition
        getf "/api/persons/%i" getPerson
        get "/api/persons" getPeople
        getf "/api/persons/%s" getPeopleByName
        
        // POST endpoints
        post "/api/persons" storePerson
        
        // PUT endpoints
        put "/api/persons" updatePerson
        
        // DELETE endpoints
        deletef "/api/persons/%i" deletePerson
    }

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message


