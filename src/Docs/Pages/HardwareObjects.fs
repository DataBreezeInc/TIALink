module Docs.Pages.HardwareObjects

open Feliz
open Elmish
open Docs.SharedView

let plugNewHardwareObject =

    let code = """

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "Your Project Name"
|> PlcProgram.addAllLanguages
|> PlcProgram.getDevice ("6ES7 510-1DJ01-0AB0/V2.9","ET200SP")
|> PlcProgram.plugNewHarwareObjects hardwareObjects
|> PlcProgram.plugNew { OrderNumber = "6ES7 131-6BF00-0BA0/V1.1"
                        Name = "30A4.1"
                        Position = 1} """
    let title = Html.text "Add a hardware object to the TIA Project."
    codedNoExampleView title code
let plugNewHardwareObjects =

    let code = """

let hardwareObjects =
    [ "6ES7 131-6BF00-0BA0/V1.1", "30A4.1"
      "6ES7 131-6BF00-0BA0/V1.1", "30A5.1"
      "6ES7 131-6BF00-0BA0/V1.1", "30A6.1"
      "6ES7 131-6BF00-0BA0/V1.1", "30A7.1"
      "6ES7 131-6BF00-0BA0/V1.1", "30A8.1"
      "6ES7 131-6BF00-0BA0/V1.1", "30A9.1"
      "6ES7 132-6BF00-0BA0/V1.1", "30A10.1"
      "6ES7 132-6BF00-0BA0/V1.1", "30A11.1"
      "6ES7 132-6BF00-0BA0/V1.1", "30A12.1"
      "6ES7 132-6BF00-0BA0/V1.1", "30A13.1"
      ]
    |> List.mapi (fun i (x,y) -> {  OrderNumber = x
                                    Name = y
                                    Position = i + 2})

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "Your Project Name"
|> PlcProgram.addAllLanguages
|> PlcProgram.getDevice ("6ES7 510-1DJ01-0AB0/V2.9","ET200SP")
|> PlcProgram.plugNewHarwareObjects hardwareObjects"""
    let title = Html.text "Add a list of hardware objects to the TIA Project."
    codedNoExampleView title code


[<ReactComponent>]
let HardwareObjectsView () =
    React.fragment [
        plugNewHardwareObject
        plugNewHardwareObjects
    ]