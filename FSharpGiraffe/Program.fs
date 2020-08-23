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
open HttpHandlers

module TerseIgnore =
    //readability trick
    let (!) a = a |> ignore

open TerseIgnore

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
        //swagger is not self generated, so we need to adjust the json manually for now..
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