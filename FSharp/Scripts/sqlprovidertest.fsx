#r "nuget:Npgsql"
#r "nuget:SQLProvider"

open Npgsql
open FSharp.Data.Sql

[<AutoOpen>]
module Settings =

    [<Literal>]
    let dbVendor = Common.DatabaseProviderTypes.POSTGRESQL

    [<Literal>]
    let connString =
        "Host=localhost;Username=postgres;Password=postgres;Database=postgres"

    [<Literal>]
    let owner = "public, admin, references"

    [<Literal>]
    let resPath = __SOURCE_DIRECTORY__

    [<Literal>]
    let indivAmount = 1000

    [<Literal>]
    let useOptTypes = Common.NullableColumnType.NO_OPTION

type sql =
    SqlDataProvider<DatabaseVendor=dbVendor, ConnectionString=connString, ResolutionPath=resPath, UseOptionTypes=useOptTypes, Owner=owner>

let ctxFactory connectionStringRuntime =
    sql.GetDataContext(connectionStringRuntime: string)
