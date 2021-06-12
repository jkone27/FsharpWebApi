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
open Saturn
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
        | _ -> app.UseGiraffeErrorHandler(errorHandler)
    ).UseHttpsRedirection()
        .UseCors(configureCors)
        //https://github.com/microsoft/OpenAPI.NET to write the file?
        //swagger is not self generated, so we need to adjust the json manually for now..
        .UseSwaggerUI(Action<_>(fun (c: Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIOptions) -> 
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")))

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

    services

let configureLogging (builder : ILoggingBuilder) =
    !builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug()

let configureWebHostBuilder (webHostBuilder : IWebHostBuilder) =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    webHostBuilder
        .UseContentRoot(contentRoot)
        .UseWebRoot(webRoot)

let app = application {
    use_router webApp
    logging configureLogging
    service_config configureServices
    app_config configureApp
    use_static "static"
    use_cors "cors_policy" configureCors
    webhost_config configureWebHostBuilder
}

run app