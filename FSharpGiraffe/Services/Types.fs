namespace Services

open FSharp.Data
open SwaggerProvider
open FSharp.Data.Sql



[<AutoOpen>]
module ProvidedTypes =

    // uses F# SQLProvider with PGSQL connection string for docker compose
    // https://fsprojects.github.io/SQLProvider/core/postgresql.html
    let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL
    let [<Literal>] connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test"

    [<Literal>]
    let owner = "dbo"

    [<Literal>]
    let indivAmount = 1000

    [<Literal>]
    let useOptTypes = Common.NullableColumnType.NO_OPTION

    type sqlProvided =
        PostgreSql.SqlDataProvider<
            DatabaseVendor = Common.DatabaseProviderTypes.POSTGRESQL,
            ConnectionStringName = "",
            ConnectionString=connString, 
            IndividualsAmount = indivAmount,
            UseOptionTypes=useOptTypes, 
            Owner=owner>


    let ctxFactory connectionStringRuntime = 
        sqlProvided.GetDataContext(connectionStringRuntime: string)

    type PetsEndpointProvider = OpenApiClientProvider<"https://petstore.swagger.io/v2/swagger.json">
    
    let petsClientFactory httpClient = 
        PetsEndpointProvider.Client(httpClient=httpClient)

[<CLIMutable>]
type PersonDto = { Name: string; Age : int; Id: int }
    with static member Map(person : sqlProvided.dataContext.``dbo.personsEntity`` ) = 
            { Name = person.Name; Age = person.Age; Id = person.Id }

type AppSettingsProvider = JsonProvider<"appsettings.json">

type AppSettings = AppSettingsProvider.Root

type OpenApiSpecProvider = JsonProvider<"openApi.json">

