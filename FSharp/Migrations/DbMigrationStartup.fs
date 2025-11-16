namespace Migrations

open Microsoft.AspNetCore.Hosting
open DbUp
open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging
open DbUp.Engine.Output
open System.Reflection
open Services

type UpgradeLog<'a>(logger: ILogger<'a>) =
    interface IUpgradeLog with
        member _.WriteError(format : string, args : System.Object[]) = 
            logger.LogError (format, args)
        member _.WriteInformation(format : string, args :  System.Object[]) = 
            logger.LogInformation (format, args)
        member _.WriteWarning(format : string, args :  System.Object[]) = 
            logger.LogWarning (format, args)

type DbMigrationStartup(settings: AppSettings, logger: ILogger<DbMigrationStartup>) =

    let connectionString : string = settings.DbConfiguration.ConnectionString
    let u1 = DeployChanges.To.PostgresqlDatabase(connectionString)
    let u2 = u1
    let upgradeEngine = 
        DeployChanges.To.PostgresqlDatabase(connectionString)
        |> _.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        |> _.WithTransaction()
        |> _.LogTo(new UpgradeLog<DbMigrationStartup>(logger))
        |> _.Build()

    interface IStartupFilter with
        member _.Configure(next) =
        
            EnsureDatabase.For.PostgresqlDatabase(connectionString)
            
            do match upgradeEngine.IsUpgradeRequired() with
                |true -> 
                    let result = upgradeEngine.PerformUpgrade()
                    do match result.Successful with
                        |true -> ()
                        |false -> logger.LogWarning((sprintf "%A" result.Error))
                |false -> ()
            
            next

