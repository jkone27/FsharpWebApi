namespace UserInterface
open Giraffe.GiraffeViewEngine


// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module UI =
    
    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "FSharpGiraffe" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "FSharpGiraffe" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
            a [ _href "/swagger" ] [ encodedText "swagger" ]
        ] |> layout