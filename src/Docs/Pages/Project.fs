module Docs.Pages.Project

open Feliz
open Elmish
open Feliz.DaisyUI
open Docs.SharedView

let project =

    let code = """@"Your/Project/Path""
|> PlcProgram.projectPath
|> PlcProgram.selectProject "Your Project Name"""
    let title = Html.text "Select a TIA Portal project. Creates a new Project if not existing."
    codedNoExampleView title code

[<ReactComponent>]
let ProjectView () =
    React.fragment [
        project
    ]