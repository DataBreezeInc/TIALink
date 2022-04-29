module Docs.Pages.DataBlock

open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView

let datablock =
    let code =
        """let section =
    [ Section.section
          Section.Static
          [ Section.memberElement
                "Meter1"
                Real
                Section.Retain
                Section.Public
                [ startValue Real 100.
                  commentElement English "Inlet water consumption Meter1"
                   ]]]

let globalDB: GlobalDB =
    { Name = "DB10"
      GlobalDBId = GlobalDBId 10
      MemoryLayout = Standard
      ProgrammingLanguage = DB
      Sections = section
      CreateTime = System.DateTime.Now
      TiaVersion = V17 }
@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.getDevice ("6ES7 510-1DJ01-0AB0/V2.9","ET200SP")
|> PlcProgram.createDataBlock globalDB"""

    let title =
        Html.text
            "Create a new DataBlock with a set of selections."

    codedWithPictureView title code "./datablock.png"


[<ReactComponent>]
let DataBlockView () =
    React.fragment [
        datablock
    ]