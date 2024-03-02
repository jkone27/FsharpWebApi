namespace Services

open FSharp.Data
open SwaggerProvider
open FSharp.Data.Sql
open System.Data


[<AutoOpen>]
module ProvidedTypes =

    [<Literal>]
    let dbVendor = Common.DatabaseProviderTypes.POSTGRESQL

    [<Literal>]
    let connString =
        "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=sqlprovider;"

    [<Literal>]
    let owner = "public, admin, references"

    [<Literal>]
    let resPath =
        __SOURCE_DIRECTORY__ + "../../../../.nuget/packages/npgsql/8.0.2/lib/net8.0"

    [<Literal>]
    let indivAmount = 1000

    [<Literal>]
    let useOptTypes = Common.NullableColumnType.NO_OPTION

    type sql =
        SqlDataProvider<DatabaseVendor=dbVendor, ConnectionString=connString, ResolutionPath=resPath, UseOptionTypes=useOptTypes, Owner=owner>

    let ctxFactory connectionStringRuntime =
        sql.GetDataContext(connectionStringRuntime: string)

    type PetsEndpointProvider = OpenApiClientProvider<"https://petstore.swagger.io/v2/swagger.json">

    let petsClientFactory httpClient = PetsEndpointProvider.Client(httpClient)

[<CLIMutable>]
type PersonDto =
    { Name: string
      Age: int
      Id: int }

    static member Map(person: sql.dataContext.``dbo.PersonsEntity``) =
        { Name = person.Name
          Age = person.Age
          Id = person.Id }

type AppSettingsProvider = JsonProvider<"appsettings.json">

type AppSettings = AppSettingsProvider.Root
