module Docs.Pages.Export

open Feliz
open Elmish
open Docs.SharedView


let exportBlock =
    let code = """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.compileProject
|> PlcProgram.exportPlcBlock "Multiply"
"""

    let title =
        Html.text "To export an Plc Block of you choice you can run following command. Get sure you compile your project first."

    codedWithPictureView title code "./exportblock.png"
let exportAllBlocks =
    let code = """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.compileProject
|> PlcProgram.exportAllPlcBlocks"""

    let title =
        Html.text "To export all your Plc Block you can run following command. Get sure you compile your project first."

    codedWithPictureView title code "./exportallblocks.png"

[<ReactComponent>]
let ExportView () =
    React.fragment [
        exportBlock
        exportAllBlocks
    ]

