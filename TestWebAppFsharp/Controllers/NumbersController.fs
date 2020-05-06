namespace Controllers

open Microsoft.AspNetCore.Mvc

open Services
       

[<Route("api")>]
type NumbersController(numbersService: INumbersService) as this = 
    inherit ControllerBase()

    [<Route("numbers")>]
    [<HttpGet>]
    member _.GetAsync() =
       async {
            let numbers = numbersService.GetNumber()
            return this.Ok(numbers) :> IActionResult
        } |> Async.StartAsTask
        

