namespace Controllers

open Microsoft.AspNetCore.Mvc
open Services


[<Route("api")>]
type PersonsController(personsRepository: PersonsRepository) as this = 
    inherit ControllerBase()


    [<Route("persons/{id}")>]
    [<HttpGet>]
    member _.GetPersonsByIdAsync([<FromRoute>]id) =
       async {
            let result = personsRepository.GetPersonById(id)
            match result with
            |Some(r) -> return this.Ok(r) :> IActionResult
            |_ -> return this.NotFound(id) :> IActionResult
        } |> Async.StartAsTask

    [<Route("persons")>]
    [<HttpPost>]
    member _.InsertPerson([<FromBody>] personDto) =
       async {
            personsRepository.InsertPerson(personDto)
            return this.Ok(personDto) :> IActionResult
        } |> Async.StartAsTask

    
    [<Route("persons/{id}")>]
    [<HttpPut>]
    member _.UpdatePerson([<FromBody>] personDto) =
       async {
            personsRepository.UpdatePerson(personDto)
            return this.Ok(personDto) :> IActionResult
        } |> Async.StartAsTask

    
    [<Route("persons/{id}")>]
    [<HttpDelete>]
    member _.DeletePerson([<FromRoute>] id) =
       async {
            personsRepository.DeletePerson(id)
            return this.StatusCode(204) :> IActionResult
        } |> Async.StartAsTask

        

