module Docs.Pages.Project

open Feliz
open Elmish
open Feliz.DaisyUI
open Docs.SharedView

let project =

    let code = """@"Your/Project/Path""
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"""
    let title = Html.text "Select a TIA Portal project. Creates a new Project if not existing."
    codedWithPictureView title code "./project.png"

[<ReactComponent>]
let ProjectView () =
    React.fragment [
        project
    ]