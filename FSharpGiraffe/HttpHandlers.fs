module HttpHandlers

open System
open Microsoft.Extensions.Logging
open Giraffe
open Services
open FSharp.Control.Tasks.V2
open UserInterface
open Microsoft.AspNetCore.Http

// ---------------------------------
// Web app
// ---------------------------------

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model = { Text = greetings }
    let view = UI.index model
    htmlView view

let loadSwaggerDefinition =
    text (OpenApiSpecProvider.GetSample().JsonValue.ToString())

let handleGetHelloWithName (name: string) =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
        let response = {|
            Text = sprintf "Hello, %s" name
        |}
        return! json response next ctx
    }

let getPerson (id : int) : HttpHandler = 
    fun next ctx -> 
    task {
        let personsRepository = ctx.GetService<PersonsRepository>()
        let response = personsRepository.GetPersonById id
        return! json response next ctx
    }

let webApp : HttpHandler =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler "world"
                routef "/hello/%s" indexHandler
                routef "/api/hello/%s" handleGetHelloWithName
                route "/swagger/v1/swagger.json" >=> loadSwaggerDefinition
                routef "/api/persons/%i" getPerson
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message


