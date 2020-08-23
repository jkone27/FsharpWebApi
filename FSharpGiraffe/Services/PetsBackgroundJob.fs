namespace Services

open System
open System.Net.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open System.Threading.Tasks
open System.Threading
open System.Linq


type PetsBackgroundJob(petsApiClient: PetsApiClient, logger: ILogger<PetsBackgroundJob>) =
    
    let callback state =
        let result = petsApiClient.GetAsync() |> Async.RunSynchronously
        logger.LogInformation("Retrieved Pets! {0}", result.Count())
    
    let doWork = new TimerCallback(callback)

    let timer = new Timer(doWork, null, TimeSpan.FromDays(1.0), TimeSpan.FromSeconds(5.0))

    interface IHostedService with
        member _.StartAsync(_) =        
            logger.LogInformation("Timed Hosted Service running.")
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5.0)) |> ignore
            Task.CompletedTask

        member _.StopAsync(_) =
            logger.LogInformation("Job is stopping.")
            do match timer with
               |null -> ()
               |t -> t.Change(TimeSpan.Zero, TimeSpan.Zero) |> ignore
            Task.CompletedTask

   
