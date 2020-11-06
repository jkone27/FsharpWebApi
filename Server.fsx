#r "nuget:Suave"
//https://github.com/forki/suaveondocker
//https://suave.io/api.html

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open System.Net

let app =
  choose
    [ GET >=> choose
        [ path "/hello" >=> OK "Hello GET"
          path "/goodbye" >=> OK "Good bye GET" ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] 
    ]

let cfg = { 
    defaultConfig 
        with bindings = [ 
            HttpBinding.createSimple HTTP "127.0.0.1" 5000
        ] 
    }

startWebServer cfg app