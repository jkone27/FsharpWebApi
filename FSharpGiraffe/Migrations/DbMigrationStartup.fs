namespace Migrations

open Microsoft.AspNetCore.Hosting
open DbUp
open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging
open DbUp.Engine
open System.Reflection
open Services

type UpgradeLog<'a>(logger: ILogger<'a>) =
    interface Output.IUpgradeLog with        
        member _.LogDebug(format: string, args: obj array): unit = 
            logger.LogDebug(format, args)
        member _.LogError(format: string, args: obj array): unit = 
            logger.LogError(format, args)
        member _.LogError(ex: exn, format: string, args: obj array): unit = 
            logger.LogError(ex, format, args)
        member _.LogInformation(format: string, args: obj array): unit = 
            logger.LogInformation(format, args)
        member _.LogTrace(format: string, args: obj array): unit = 
            logger.LogTrace(format, args)
        member _.LogWarning(format: string, args: obj array): unit = 
            logger.LogWarning(format, args)
        
type DbMigrationStartup(settings: AppSettings, logger: ILogger<DbMigrationStartup>) =

    let connectionString : string = settings.DbConfiguration.ConnectionString
    let upgradeEngine = 
        DeployChanges.To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogTo(new UpgradeLog<DbMigrationStartup>(logger))
            .Build()

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