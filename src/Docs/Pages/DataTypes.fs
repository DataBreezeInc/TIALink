module Docs.Pages.DataTypes

open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView


let dataTypes =

    let code = """
    type DataType =
    | Bool
    | Byte
    | Word
    | DWord
    | Int
    | DInt
    | UInt
    | UDInt
    | Real
    | S5Time
    | Time
    | Void
    | Struct
    | Custom of string"""
    let title =
        Html.text "TIALink supports following DataTypes"
    codedNoExampleView title code

[<ReactComponent>]
let DataTypesView () =
    React.fragment [
        dataTypes
    ]
