module Docs.Pages.Languages

open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView

let addLanguage =
    let code = """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.addLanguage German"""
    let title =
        Html.text "Add one language to the TIA Project."

    codedWithPictureView title code "./language.png"
let allLanguages =
    let code = """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.addAllLanguages"""

    let title =
        Html.text "Configure all languages to the TIA Project."

    codedWithPictureView title code "./languages.png"

[<ReactComponent>]
let LanguagesView () =
    React.fragment [
        addLanguage
        allLanguages
    ]

