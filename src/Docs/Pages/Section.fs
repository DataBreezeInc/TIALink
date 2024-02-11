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
        Html.text "TIALink supports following Sections"
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

let sections =

    let code = """let sections =
    [ Section.section
          Section.Static
          [ Section.memberElement
                "Meter1"
                Real
                Section.Retain
                Section.Public
                [ startValue Real 100.
                  commentElement English "Inlet water consumption Meter1"
                   ]]]"""
    let title =
        Html.text "Creates a section XML Element"
    codedWithPictureView title code "./datablock.png"

[<ReactComponent>]
let SectionView () =
    React.fragment [
        section
        accessibility
        remanence
        sections
    ]
