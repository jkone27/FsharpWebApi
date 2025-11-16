# Fsharp Web Api

Compares two almost identical APIs in F# and C#.

Features:
* CRUD controller to access a simple Persons Entity.
* Background Job to call petstore api via apiClient and print a message to log

A [medium article](https://jkone27-3876.medium.com/comparing-f-and-c-in-real-life-asp-net-core-aebd32812ce3) (outdated, but here for reference) describing the repository setup.

## C# (MVC)

Uses [EF core](https://learn.microsoft.com/en-gb/ef/) PGSQL, ef migrations

## F# (MVC)

Uses Swashbuckle for swagger and also [FSharp.Data](https://fsprojects.github.io/FSharp.Data/) to read `appsetting.json`, and [SwaggerTypeProvider](https://fsprojects.github.io/SwaggerProvider/#/) for connecting to petstore test api, and [Fsharp.Data.SqlProvider](https://fsprojects.github.io/SQLProvider/core/postgresql.html) for an "EF like" ~ experience for SQL query, and [DBup]() for migrations.

## F# Giraffe

Shows a similar api just using Giraffe endpoints. a more modern version could use Falco or OxPecker, which are modern web frameworks for F#, this was an old project ported to work for PGSQL and NET8 mostly, feel free to make a PR and add other F# variants!


## Getting started

You can run this repository inside a dev container, to facilitate testing against PGSQL.

* run `dotnet tool restore`

if you run locally (not via dev containers), remember to start first the `docker-compose.yaml` file in your podman or docker desktop or other container engine solution you might be using locally.

* `dotnet build` to build and compile all projects.
* `dotnet run --project CSharp` to run the C# variant
* `dotnet run --project FSharp` to run the F# variant
* `dotnet run --project FSharpGiraffe` to run the F# variant with Giraffe - functional routing.

you can then test your endpoints via `.http` files