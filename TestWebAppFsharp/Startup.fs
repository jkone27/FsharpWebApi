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

[<AutoOpen>]
module ConfigurationExtensions = 
    type Microsoft.Extensions.Configuration.IConfiguration with 
        member _.Bind2(obj : AppSettings) : unit =
            ()
        

type Startup(configuration: IConfiguration) =

    member _.ConfigureServices(services: IServiceCollection) =
        
        services.AddSingleton<INumbersService, NumbersService>() |> ignore
        services.AddSingleton<PersonsRepository>() |> ignore
        services.AddTransient<IStartupFilter, DbMigrationStartup>() |> ignore

        //configuration using FSharp.Data type provider
        let settingsFile = "appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json";
        let config = AppSettingsProvider.Load(settingsFile)

        //bind does not work with provided types, for options need to use normal classes
        //configuration.GetSection("DbConfiguration").Bind(config.DbConfiguration)
        
        //add once and never change (just add changes in appsettings.json!!!)
        services.AddSingleton<AppSettings>(config) |> ignore
        services.AddHttpClient<PetsApiClient>() |> ignore
        services.AddHostedService<PetsBackgroundJob>() |> ignore

        //needed for swagger
        services.AddMvc() |> ignore
        services.AddSwaggerGen(fun c ->
            c.SwaggerDoc("v1", new OpenApiInfo( Title = "Persons API", Version = "v1" ))
        )  |> ignore

        services.AddControllers() |> ignore

    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseRouting() |> ignore

        app.UseAuthorization() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
            ) |> ignore

        //needed for swagger
        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun c ->
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
        ) |> ignore
