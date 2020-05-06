namespace Migrations

open Microsoft.AspNetCore.Hosting
open DbUp
open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging
open AppConfiguration
open DbUp.Engine.Output
open System.Reflection

type UgradeLog<'a>(logger: ILogger<'a>) =
    interface IUpgradeLog with
        member _.WriteError(format : string, args : System.Object[]) = 
            logger.LogError (format, args)
        member _.WriteInformation(format : string, args :  System.Object[]) = 
            logger.LogInformation (format, args)
        member _.WriteWarning(format : string, args :  System.Object[]) = 
            logger.LogError (format, args)

type DbMigrationStartup(dbConfigurationOptions: IOptions<DbConfiguration>, logger: ILogger<DbMigrationStartup>) =

    let dbConfiguration = dbConfigurationOptions.Value;
    let u1 = DeployChanges.To.SqlDatabase(dbConfiguration.ConnectionString)
    let u2 = u1.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    let upgradeEngine = u2.WithTransaction().LogTo(new UgradeLog<DbMigrationStartup>(logger)).Build()

    interface IStartupFilter with
        member _.Configure(next) = 
            
            do match upgradeEngine.IsUpgradeRequired() with
                |true -> 
                    let result = upgradeEngine.PerformUpgrade()
                    do match result.Successful with
                        |true -> ()
                        |false -> logger.LogWarning((sprintf "%A" result.Error))
                |false -> ()
            
            next

