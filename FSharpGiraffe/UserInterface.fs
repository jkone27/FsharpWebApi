namespace UserInterface
open Giraffe.ViewEngine
open Feliz
open Feliz.Bulma

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

module Constants =
    [<Literal>]
    let bulmaCdn = "https://cdn.jsdelivr.net/npm/bulma@0.9.2/css/bulma.min.css"

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


module FelizUi =
    
    open Feliz.ViewEngine
    open Feliz.Bulma.ViewEngine
    // https://bulma.io/documentation/overview/start/

        let index (model : Message) =
           
           let head = 
                Html.head [
                    prop.children [ 
                        Html.title "my awesome Saturn api"
                        Html.link [
                            prop.rel "stylesheet"
                            prop.href Constants.bulmaCdn
                        ]
                    ]      
                ]

           let body = 
                Bulma.hero [
                    Bulma.heroHead [ 
                        prop.text "test from Feliz" 
                        prop.className "title is 1"
                        color.hasBackgroundPrimary
                        color.hasTextBlack
                    ]
                    Bulma.heroBody [
                        Bulma.container [ 
                            prop.text $"YO! {model.Text}"
                            color.hasBackgroundBlack
                            color.hasTextPrimary
                        ]
                    ]
                ]

           Html.html [
               prop.children [ 
                   head
                   body
               ]
           ]
           
    

