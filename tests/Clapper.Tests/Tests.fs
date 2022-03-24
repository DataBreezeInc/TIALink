open System.IO

open Siemens.Engineering
open Clapper  

let tiaPortal = new TiaPortal(TiaPortalMode.WithoutUserInterface)
let projects = tiaPortal.Projects
let path = @"C:\Users\TimForkmann\Documents\Automatisierung\" 

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

let tagTableList = "ESA Kuwait Tag List"

path
|> PlcProgram.projectPath 
|> PlcProgram.activateUI
|> PlcProgram.selectProject "ESA Kuwait"
|> PlcProgram.getDevice ("ET200SP","6ES7 510-1DJ01-0AB0/V2.9")
|> PlcProgram.plugNewHarwareObjects hardwareObjects
|> PlcProgram.addTagTable tagTableList
|> PlcProgram.addTags (tags,tagTableList) 


// let targetDir = DirectoryInfo(path)  

// let projectPath = FileInfo(path + projectName + @"\" + projectName + ".ap17")

// let myProject = projects.OpenWithUpgrade(projectPath)


// let device =
//     { Project = myProject
//       OrderNumber = "6ES7 510-1DJ01-0AB0/V2.9"
//       DeviceName = "ET200SP" }


// let plcDevice = createOrGetDevice device
// // for item in plcDevice.Items do
// //   printfn "Items %A" item.Addresses

// // for hwId in plcDevice.HwIdentifiers do
// //   printfn "hwId %A" hwId.Identifier
// //   printfn "hwId %A" hwId.HwIdentifierControllers

// let plcSoftware = getPlcSoftware plcDevice
// printfn "PlcSoftwareName %A" plcSoftware.Name
// printfn "TagTableGroup Name %A" plcSoftware.TagTableGroup.Name

// let tagTable = getTagTable plcSoftware "KuwaitTags"


// // let tagTable = getTagTable plcSoftware "Test Tag Table"
// let tags =tagTable.Tags 
// for tag in tags do 
    
//     printfn "type %A" tag.DataTypeName
//     printfn "Comment %A" tag.Comment.Items.[0].Text
//     printfn "LogicalAddress %A" tag.LogicalAddress
// // for tagTable in plcSoftware.TagTableGroup.TagTables do
// //     printfn "tableName %A"tagTable.Name
// //     tagTable.Tags
// myProject.Save()
// myProject.Close()
// tiaPortal.Dispose()
