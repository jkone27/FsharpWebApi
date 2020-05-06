namespace TestWebAppFsharp

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Services
open Microsoft.OpenApi.Models

type Startup() =

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddControllers() |> ignore
        services.AddScoped<INumbersService, NumbersService>() |> ignore
        services.AddScoped<PersonsRepository>() |> ignore

        //needed for swagger
        services.AddMvc() |> ignore
        services.AddSwaggerGen(fun c ->
            c.SwaggerDoc("v1", new OpenApiInfo( Title = "My API", Version = "v1" ))
        )  |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
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
