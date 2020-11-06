# FsharpWebApi

Compares two almost identical APIs in F# and C#.

Features:
* CRUD controller to access a simple Persons Entity.
* Background Job to call petstore api via apiClient and print a message to log

A [medium article](https://jkone27-3876.medium.com/comparing-f-and-c-in-real-life-asp-net-core-aebd32812ce3) describing the repository

## C#

Uses EF core SQL server, ef migrations

## F# 

Uses Swashbuckle for swagger and also FSharp.Data to read appsetting.json, and SwaggerTypeProvider for connecting to petstore test api, and Fsharp.Data.SqlProvider for an EF like experience for SQL query, and DBup for migrations.

## C# Friendlier

Uses ! at the beginning of constructs which discard the value, to avoid doing the more verbose (and very different from C#) |> ignore.

```fsharp
module TerseIgnore =
    //readability trick
    let (!) a = a |> ignore
```
