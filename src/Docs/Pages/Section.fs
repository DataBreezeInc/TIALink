module Docs.Pages.Section

open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView

let section =

    let code = """
    type Section =
        | Input
        | InOut
        | Output
        | Return
        | Temp
        | Constant
        | Static
        | NoSection"""
    let title =
        Html.text "Clapper supports following Sections"
    codedNoExampleView title code

let accessibility =

    let code = """
    type Accessibility =
        | Public
        | Private
        | NoAccessibility"""
    let title =
        Html.text "Following Accessibilities are supported"
    codedNoExampleView title code
let remanence =

    let code = """
    type Remanence =
        | Retain
        | NoRemanence"""
    let title =
        Html.text "Following Remanences are supported"
    codedNoExampleView title code

[<ReactComponent>]
let SectionView () =
    React.fragment [
        section
        accessibility
        remanence
    ]