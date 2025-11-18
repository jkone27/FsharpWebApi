# F# Web Api

![image](https://github.com/user-attachments/assets/6c1d2d57-603f-41f2-9deb-52089dff576a)

Compares two almost identical APIs in F# and C#, in NET10 (LTS).

Features:
* CRUD controller to access a simple Persons Entity.
* Background Job to call petstore api via apiClient and print a message to log

A [medium article](https://jkone27-3876.medium.com/comparing-f-and-c-in-real-life-asp-net-core-aebd32812ce3) (outdated, but here for reference) describing the repository setup.

## C# (MVC)

Uses [EF core](https://learn.microsoft.com/en-gb/ef/) PGSQL, ef migrations

## F# (MVC)

⚡️💙🦔 Uses Swashbuckle for swagger and also [FSharp.Data](https://fsprojects.github.io/FSharp.Data/) to read `appsetting.json`, and [SwaggerTypeProvider](https://fsprojects.github.io/SwaggerProvider/#/) for connecting to petstore test api, and [Fsharp.Data.SqlProvider](https://fsprojects.github.io/SQLProvider/core/postgresql.html) for an "EF like" ~ experience for SQL query, and [DBup]() for migrations.

## F# Giraffe

 Shows a similar api just using [Giraffe endpoints](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#endpoint-routing) and [Saturn](https://saturnframework.org/explanations/endpoint-routing.html). A more "modern" variant could make use of the wonderful [Falco](https://www.falcoframework.com/) or [OxPecker](https://lanayx.github.io/Oxpecker/) one of the most performant frameworks in benchmarks. Those listed last are modern web frameworks for F#, this is an old project ported to work for PGSQL and NET10 mostly, it might evolve further in the future.

## Getting started

You can run this repository inside a dev container, to facilitate testing against PGSQL.

* run `dotnet tool restore`
* run `dotnet dev-certs https` if prompted for enabling dev certificates

if you run locally (not via dev containers), remember to start first the `docker-compose.yaml` file in your podman or docker desktop or other container engine solution you might be using locally.

* `dotnet build` to build and compile all projects.
* `dotnet run --project CSharp` to run the C# variant

note on F# type providers, the SQL type provider depends on pre-existing schema to compile your types,
so ideally when working in code/editing, you should first update your SQL, run dbup, or efcore migrate.
this is a bit counter intuitive, but forces your local code to be in sync with your local db. interesting? right. 

for a quick setup u can either run C# first, or you can run `cd FSharp/Scripts && dotnet fsi sqlprovidertest.fsx` or create the schema yourself by connecting to pgsql locally.

* `dotnet run --project FSharp` to run the F# variant
* `dotnet run --project FSharpGiraffe` to run the F# variant with Giraffe - functional routing.

you can then test your endpoints via `.http` files
