module Docs.Pages.Save

open Feliz
open Elmish
open Docs.SharedView

let save =
    let code = """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.compileProject
|> PlcProgram.saveAndClose"""

    let title =
        Html.text "Make sure your compile your project before you save your project."

    codedNoExampleView title code

[<ReactComponent>]
let SaveView () =
    React.fragment [
        save
    ]

