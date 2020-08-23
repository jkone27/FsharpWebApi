module FSharpGiraffe.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Services
open Migrations
open Swashbuckle.AspNetCore.SwaggerUI
open FSharp.Control.Tasks.V2

module TerseIgnore =
    //readability trick
    let (!) a = a |> ignore

open TerseIgnore
open Microsoft.AspNetCore.Http

// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open GiraffeViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "FSharpGiraffe" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "FSharpGiraffe" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
            a [ _href "/swagger" ] [ encodedText "swagger" ]
        ] |> layout

// ---------------------------------
// Web app
// ---------------------------------

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model     = { Text = greetings }
    let view      = Views.index model
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

let webApp =
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

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.EnvironmentName with
    | "Development" -> app.UseDeveloperExceptionPage()
    | _ -> app.UseGiraffeErrorHandler(errorHandler))
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        //.UseSwagger(fun x -> x.RouteTemplate <- "")
        .UseSwaggerUI(Action<_>(fun c -> c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")))
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    
    !services.AddCors()
    !services.AddGiraffe()
    !services.AddSingleton<PersonsRepository>()
    !services.AddTransient<IStartupFilter, DbMigrationStartup>()

    //configuration using FSharp.Data type provider
    let settingsFile = "appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json"
    let config = AppSettingsProvider.Load(settingsFile)

    //bind does not work with provided types, for options need to use normal classes
    //!configuration.GetSection("DbConfiguration").Bind(config.DbConfiguration)
          
    //add once and never change (just add changes in appsettings.json!!!)
    !services.AddSingleton<AppSettings>(config)
    !services.AddHttpClient<PetsApiClient>()
    !services.AddHostedService<PetsBackgroundJob>()

let configureLogging (builder : ILoggingBuilder) =
    !builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug()

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                !webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    )
        .Build()
        .Run()
    0