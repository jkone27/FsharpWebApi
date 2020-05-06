namespace Services

type INumbersService = 
    abstract member GetNumber: unit -> int

type NumbersService() =
    let r = System.Random()
    interface INumbersService with
        member _.GetNumber() = r.Next(1,10)

