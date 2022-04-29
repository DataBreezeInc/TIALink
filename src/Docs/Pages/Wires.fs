module Docs.Pages.Badge

open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView

let wireTypes =

    let code =
        """type WireType =
    | IdentCon of UId
    | PowerRail
    | OpenCon of UId"""

    let title = Html.text "Clapper supports following WireTypes."
    codedNoExampleView title code
let wire =

    let code =
        """ type Wire =
        { UId: UId
          Name: string
          NameUId: UId
          WireType: WireType }"""

    let title = Html.text "A wire contain following elements."
    codedNoExampleView title code

let wireElement =
    let example =
        Daisy.badge [
            badge.sm
            badge.success
            prop.text "Success badge"
        ]
        |> Html.div

    let code = """Wires.wireElement
                            {   UId = UId 26
                                Name = "in1"
                                NameUId = UId 24
                                WireType = Wires.IdentCon(UId 21) }  "
]"""
    let title =
        Html.text
            "Creates a Mulitply Element with comment, titel and two inputs and one output."

    codedWithPictureView title code "./wireelement.png"

[<ReactComponent>]
let WiresView () =
    React.fragment [
        wireTypes
        wire
        wireElement
    ]