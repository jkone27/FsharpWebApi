namespace Services

open System
open System.Net.Http


type PetsApiClient(httpClient: HttpClient, appSettings: AppSettings) =
    
    let baseAddress = appSettings.ApiClients.BaseAddress
    do httpClient.BaseAddress <- Uri(baseAddress)
    let client = petsClientFactory(httpClient)
    
    member _.GetAsync() = client.FindPetsByStatus([|"available"|])

