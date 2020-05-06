[<AutoOpen>]
module Types

open FSharp.Data.Sql

[<Literal>]
let connectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

type sql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, connectionString>

let ctx = sql.GetDataContext()

[<CLIMutable>]
type PersonDto = { Name: string; Age : int; Id: int }
    with static member Map(person : sql.dataContext.``dbo.PersonsEntity``) = 
            { Name = person.Name; Age = person.Age; Id = person.Id }
