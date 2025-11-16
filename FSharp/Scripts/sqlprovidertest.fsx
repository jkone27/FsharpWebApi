#r "nuget: SQLProvider.PostgreSql"
#r "nuget: Npgsql"
open FSharp.Data.Sql
open System


// uses F# SQLProvider with PGSQL connection string for docker compose
// https://fsprojects.github.io/SQLProvider/core/postgresql.html
let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL
let [<Literal>] connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test"

// the resolution path needs to be aware of #r nuget and the fact that this is a "temp F# project" created by executing an .fsx script
// [Loading /Users/admin/.packagemanagement/nuget/Projects/78008--86fcbb22-7111-4bc7-82d3-2771e40a54f2/Project.fsproj.fsx]
// module FSI_0003.Project.fsproj

// e.g. ~/.nuget/packages/Npgsql/9.0.4/lib/net8.0/Npgsql.dll but type provider should find it on its own
let [<Literal>] resPath = "" // THIS WORKED FOR ME
let [<Literal>] owner = "dbo" // or "dbo" for MSSQL

type SqlProvided =
    PostgreSql.SqlDataProvider<
        dbVendor,
        connString,
        "", 
        resPath,
        1000,
        Common.NullableColumnType.OPTION,
        owner>

let ctx = SqlProvided.GetDataContext()

let c = ctx.CreateConnection()
c.Open()

let sql (txt: string) = txt

// add a sample Person test table PGSQL
let sqlCreateTable =
    sql """
    CREATE SCHEMA IF NOT EXISTS dbo;

    CREATE TABLE IF NOT EXISTS dbo.persons (
        id SERIAL PRIMARY KEY,
        name VARCHAR(20),
        age INT
    );
    """

let dbc = c.CreateCommand()
dbc.CommandText <- sqlCreateTable
dbc.ExecuteNonQuery() |> ignore

printfn "Table created successfully (or already existing!)"

let newPerson = ctx.Dbo.Persons.Create()
newPerson.Age <- 12 |> Some
let johnId = Guid.NewGuid().ToString().Substring(0, 4)
newPerson.Name <- $"John_{johnId}" |> Some

ctx.SubmitUpdates()

printfn $"created John NR: {johnId}"

for p in ctx.Dbo.Persons do
    printfn $"name: {p.Name}, age: {p.Age}, id {p.Id}"

c.Close()

