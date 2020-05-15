namespace Services

open FSharp.Data
open SwaggerProvider
open FSharp.Data.Sql



[<AutoOpen>]
module ProvidedTypes =

    [<Literal>]
    let private connectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

    type sql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, connectionString>

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

