module Docs.Pages.Compile

open Feliz
open Elmish
open Docs.SharedView

let compile =
    let code = """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.compileProject"""

    let title =
        Html.text "Compile your project and receive your compileresults. Make sure your compile your project before you save your project."

    codedWithPictureView title code "./compileresult.png"

[<ReactComponent>]
let CompileView () =
    React.fragment [
        compile
    ]

