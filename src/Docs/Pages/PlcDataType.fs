module Docs.Pages.PlcDataType

open Browser.Types
open Fable.Core
open Fable.Core.JS
open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView

let attributeInfo =

    let code =
        """type AttributeInfo =
        | ExternalAccessible
        | ExternalVisible
        | ExternalWritable
        | SetPoint
"""

    let title = Html.text "Currently the following attributeInfos are supported."
    codedNoExampleView title code

let globalDb =

    let code =
        """type PlcDataType =
        { Name: string
          Number: int
          DataTypeId: DataTypeId
          Sections: seq<Xml>
          CreationTime: DateTime }
"""

    let title = Html.text "PlcDataType contains following elements."
    codedNoExampleView title code


[<ReactComponent>]
let PlcDataTypeView () =
    React.fragment [
        attributeInfo
        globalDb
    ]