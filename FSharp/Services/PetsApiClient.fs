namespace Services

open System
open System.Net.Http


type PetsApiClient(httpClient: HttpClient, appSettings: AppSettings) =
    
    let baseAddress = appSettings.ApiClients.BaseAddress
    do httpClient.BaseAddress <- Uri(baseAddress)
    let client = petsClientFactory(httpClient)
    
    member _.GetAsync() = 
        async {
            let! pets = client.FindPetsByStatus([|"available"|]) |> Async.AwaitTask

            return pets |> Seq.take(10)
        }
        

