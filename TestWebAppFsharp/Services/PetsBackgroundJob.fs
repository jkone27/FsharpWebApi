namespace Services

open System
open System.Net.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open System.Threading.Tasks
open System.Threading


type PetsBackgroundJob(petsApiClient: PetsApiClient, logger: ILogger<PetsBackgroundJob>) =
    
    let mutable timer : Timer option = None

    let callback state =
        let result = petsApiClient.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult()
        logger.LogInformation("Retrieved Pets! {0}", result.Length)
    
    let doWork = new TimerCallback(callback)

    interface IHostedService with
        member _.StartAsync(_) =        
            logger.LogInformation("Timed Hosted Service running.")
            timer <- (new Timer(doWork, None, TimeSpan.Zero, TimeSpan.FromSeconds(5.0)) |> Some)
            Task.CompletedTask

        member _.StopAsync(_) =
            logger.LogInformation("Job is stopping.")
            do match timer with
               |Some(t) -> t.Change(Timeout.Infinite, 0) |> ignore
               |None -> ()
            Task.CompletedTask

   
