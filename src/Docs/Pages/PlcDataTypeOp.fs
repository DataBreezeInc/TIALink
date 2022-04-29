module Docs.Pages.PlcDataTypeOp

open Feliz
open Elmish
open Feliz.DaisyUI
open Docs.SharedView

let createAndImportPlcDataType =

    let code =
        """let plcDataType: PlcDataType =
    { Name = "FREQ_COUNTER"
      Number = 71
      DataTypeId = DataTypeId 0
      Sections = sectionsFREQ_COUNTER
      CreationTime = System.DateTime.Now }

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.createAndImportPlcDataType ("FREQ_COUNTER", V17, plcDataType)"""

    let title = Html.text "Create and import a new PLC data type."
    codedWithPictureView title code "./plcdatatypeop.png"

[<ReactComponent>]
let PlcDataTypeOpView () =
    React.fragment [ createAndImportPlcDataType ]