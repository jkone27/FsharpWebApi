#nowarn "20" 
// ignore FS0020 on this file, since we use aspnet DI, no need to add |> ignore every time
namespace TestWebAppFsharp

module Program =
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Configuration
    open Microsoft.Extensions.Options
    open Services
    open Microsoft.OpenApi.Models
    open Microsoft.AspNetCore.Http
    open Migrations
    open System

    let configureServices (services: IServiceCollection) =
        services.AddSingleton<PersonsRepository>()
        services.AddTransient<IStartupFilter, DbMigrationStartup>()

        //configuration using FSharp.Data type provider
        let settingsFile = "appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json"
        let config = AppSettingsProvider.Load(settingsFile)

        //bind does not work with provided types, for options need to use normal classes
        //!configuration.GetSection("DbConfiguration").Bind(config.DbConfiguration)
        
        //add once and never change (just add changes in appsettings.json!!!)
        services.AddSingleton<AppSettings>(config)
        services.AddHttpClient<PetsApiClient>()
        services.AddHostedService<PetsBackgroundJob>()

        //needed for swagger
        services.AddMvc()
        services.AddSwaggerGen(fun c ->
            c.SwaggerDoc("v1", new OpenApiInfo( Title = "Persons API", Version = "v1" ))
        )

        services.AddControllers()

    let configureApp (app : IApplicationBuilder) (env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage()
            ()

        app.UseHttpsRedirection()
        app.UseRouting()

        app.UseAuthorization()

        //needed for swagger
        app.UseSwagger()
        app.UseSwaggerUI(fun c ->
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
        )

    let configureWebApplication () =
        let builder = WebApplication.CreateBuilder()

        let services = builder.Services
        let Configuration = builder.Configuration

        configureServices services

        let env = builder.Environment
        let app = builder.Build()

        configureApp app env

        app.MapControllers()
        app.MapGet("/", fun context -> context.Response.WriteAsync("Welcome to F#!"))

        app


    [<EntryPoint>]
    let main _ =
        configureWebApplication().Run()
        0
