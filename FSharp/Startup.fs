namespace TestWebAppFsharp

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open Services
open Microsoft.OpenApi.Models
open Migrations

module TerseIgnore =
    //readability trick
    let (!) a = a |> ignore

open TerseIgnore

type Startup(configuration: IConfiguration) =

    member _.ConfigureServices(services: IServiceCollection) =

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

        //needed for swagger
        !services.AddMvc()
        !services.AddSwaggerGen(fun c ->
            c.SwaggerDoc("v1", new OpenApiInfo( Title = "Persons API", Version = "v1" ))
        )

        !services.AddControllers()

    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            !app.UseDeveloperExceptionPage()

        !app.UseHttpsRedirection()
        !app.UseRouting()

        !app.UseAuthorization()

        !app.UseEndpoints(fun endpoints ->
            !endpoints.MapControllers()
            !endpoints.MapGet("/", fun context -> context.Response.WriteAsync("Welcome to F#!"))
            )

        //needed for swagger
        !app.UseSwagger()
        !app.UseSwaggerUI(fun c ->
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
        )
