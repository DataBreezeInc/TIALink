open Siemens.Engineering
open Clapper  

let tiaPortal = new TiaPortal(TiaPortalMode.WithoutUserInterface)
let projects = tiaPortal.Projects
let path = @"Your path" 

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
let tags =
    [ "E0.0", Bool, "air blower 1","%10.1"
      "E0.1", Bool, "air blower 2","%10.2"
      "E0.2", Bool, "air blower 3","%10.3"
      ]
    |> List.mapi (fun i (x,y,z,a) ->    {
                                        Name = x
                                        DataType = y
                                        Comment =z
                                        Address = a
                                })  

let tagTableList = "Tag List Name"

path
|> PlcProgram.projectPath 
|> PlcProgram.activateUI
|> PlcProgram.selectProject "ESA Kuwait"
|> PlcProgram.getDevice ("ET200SP","6ES7 510-1DJ01-0AB0/V2.9")
|> PlcProgram.plugNewHarwareObjects hardwareObjects
|> PlcProgram.addTagTable tagTableList
|> PlcProgram.addTags (tags,tagTableList) 
|> PlcProgram.saveAndClose 