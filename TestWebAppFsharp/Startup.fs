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
open AppConfiguration
open Migrations

type Startup(configuration: IConfiguration) =

    member _.ConfigureServices(services: IServiceCollection) =
        services.AddControllers() |> ignore
        services.AddSingleton<INumbersService, NumbersService>() |> ignore
        services.AddSingleton<PersonsRepository>() |> ignore
        services.AddTransient<IStartupFilter, DbMigrationStartup>() |> ignore

        //configuration
        let configurationFunction = 
            new System.Action<_>(
                fun (opt :DbConfiguration) ->  
                    configuration.GetSection("DbConfiguration").Bind(opt)) 
        
        services.Configure<DbConfiguration>(configurationFunction) |> ignore

        //needed for swagger
        services.AddMvc() |> ignore
        services.AddSwaggerGen(fun c ->
            c.SwaggerDoc("v1", new OpenApiInfo( Title = "Persons API", Version = "v1" ))
        )  |> ignore

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
