namespace Controllers

open Microsoft.AspNetCore.Mvc
open Services
open FSharp.Control.Tasks.V2


[<ApiController>]
[<Route("api")>]
type PersonsController(personsRepository: PersonsRepository) as this =
    inherit ControllerBase()


    [<Route("persons/{id}")>]
    [<HttpGet>]
    member _.GetPersonsByIdAsync([<FromRoute>]id) =
        task {
            let result = personsRepository.GetPersonById(id) 

            let httpResult = 
                match result with
                |Some(r) ->  this.Ok(r) :> IActionResult
                |_ ->  this.NotFound(id) :> IActionResult

            return httpResult
        }

    [<Route("persons")>]
    [<HttpPost>]
    member _.InsertPerson([<FromBody>] personDto) =
       task {
            personsRepository.InsertPerson(personDto)
            return this.Ok(personDto)
        }

    
    [<Route("persons")>]
    [<HttpPut>]
    member _.UpdatePerson([<FromBody>] personDto) =
       task {
            personsRepository.UpdatePerson(personDto)
            return this.Ok(personDto)
        }

    
    [<Route("persons/{id}")>]
    [<HttpDelete>]
    member _.DeletePerson([<FromRoute>] id) =
       task {
            personsRepository.DeletePerson(id)
            return this.StatusCode(204)
        }

        

