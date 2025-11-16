#r "nuget:Npgsql"
open Npgsql

// verify connection string
let connectionString =
    "Host=localhost;Username=postgres;Password=postgres;Database=postgres"

let dataSource = NpgsqlDataSource.Create(connectionString)

let command = dataSource.CreateCommand("SELECT 1")
let reader = command.ExecuteReader()

reader.Read()

reader.GetInt32(0) |> printfn "%i"

reader.Dispose()

command.Dispose()

dataSource.Dispose()
