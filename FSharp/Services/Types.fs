namespace Services

open FSharp.Data
open SwaggerProvider
open FSharp.Data.Sql
open Microsoft.Extensions.Logging.Abstractions


[<AutoOpen>]
module ProvidedTypes =

    let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL

    let [<Literal>] connString ="Host=127.0.0.1;Database=yourdatabase;Username=yourusername;Password=yourpassword"

    let [<Literal>] owner = "public, admin, references"

    let [<Literal>] resPath = "~/.nuget/packages/npgsql/8.0.0"

    let [<Literal>] indivAmount = 1000

    let [<Literal>] useOptTypes  = true

    type sql =
        SqlDataProvider<
            dbVendor,
            connString,
            "",         //ConnectionNameString can be left empty 
            resPath,
            indivAmount,
            useOptTypes,
            owner>

    let ctxFactory connectionStringRuntime = 
        sql.GetDataContext(connectionStringRuntime: string)

    type PetsEndpointProvider = OpenApiClientProvider<"https://petstore.swagger.io/v2/swagger.json">
    
    let petsClientFactory httpClient = 
        PetsEndpointProvider.Client(httpClient)

[<CLIMutable>]
type PersonDto = { Name: string; Age : int; Id: int }
    with static member Map(person : sql.dataContext.``dbo.PersonsEntity``) = 
            { Name = person.Name; Age = person.Age; Id = person.Id }

type AppSettingsProvider = JsonProvider<"appsettings.json">

type AppSettings = AppSettingsProvider.Root

