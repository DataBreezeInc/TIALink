namespace Clapper

open System
open System.IO
open Siemens.Engineering
open Siemens.Engineering.HW
open Siemens.Engineering.HW.Features
open Siemens.Engineering.SW
open Siemens.Engineering.SW.Tags

open Siemens.Engineering.SW.Types
open Siemens.Engineering.Hmi

type HardwareObject =
    { OrderNumber: string
      Name: string
      Position: int }

type DataType =
    | Bool
    member this.getName =
        match this with
        | Bool -> "Bool"


type Tag =
    { Name: string
      DataType: DataType
      Comment: string
      Address: string }

[<RequireQualifiedAccess>]
module PlcProgram =
    type PlcProps =
        private
            { ExistingTiaPortalConnection: TiaPortal option
              Project: Project option
              Device: Device option
              PlcSoftware : PlcSoftware option
              ProjectName: string
              UserInterface: bool
              ProjectPath: string
              DeviceItems: DeviceItem []
              TagTableList: PlcTagTable [] }

    let private defaultProps () =
        { ExistingTiaPortalConnection = None
          Project = None
          Device = None
          PlcSoftware = None
          UserInterface = false
          ProjectPath = ""
          ProjectName = ""
          DeviceItems = [||]
          TagTableList = [||] }

    let projectPath projectPath =
        { defaultProps () with ProjectPath = projectPath }

    let activateUI (props: PlcProps) = { props with UserInterface = true }

    let private getTiaPortal (props: PlcProps) =
        printfn $"Opening TiaPortal..."

        match props.ExistingTiaPortalConnection with
        | Some tiaPortal -> tiaPortal
        | None ->
            if props.UserInterface then
                new TiaPortal(TiaPortalMode.WithUserInterface)
            else
                new TiaPortal(TiaPortalMode.WithoutUserInterface)

    let selectProject projectName (props: PlcProps) =
        let tiaPortal = getTiaPortal props

        printfn $"Opening Project {projectName}..."

        let targetDir =
            if not (Directory.Exists(props.ProjectPath)) then
                Directory.CreateDirectory(props.ProjectPath)
            else
                DirectoryInfo(props.ProjectPath)

        let projectPath =
            FileInfo(
                props.ProjectPath
                + projectName
                + @"\"
                + projectName
                + ".ap17"
            )

        let project =
            try
                tiaPortal.Projects.Create(targetDir, projectName)
            with
            | _ ->
                printfn "Project exists already"
                tiaPortal.Projects.OpenWithUpgrade(projectPath)

        { props with
            ExistingTiaPortalConnection = Some tiaPortal
            ProjectName = projectName
            Project = Some project }

    let private deviceExist ((project: Project), deviceName) =
        project.Devices
        |> Seq.exists (fun device -> device.Name = deviceName)

    let private findDevice ((project: Project), deviceName) =
        project.Devices
        |> Seq.find (fun device -> device.Name = deviceName)


    let private getPlcSoftware (device: Device) =
        let cpuDevice =
            device.DeviceItems
            |> Seq.find (fun deviceItem -> deviceItem.Classification = DeviceItemClassifications.CPU)

        let softwareContainer = cpuDevice.GetService<SoftwareContainer>()
        softwareContainer.Software :?> PlcSoftware
    let getDevice (orderNumber: string, deviceName: string) (props: PlcProps) =
        match props.Project with
        | Some project ->
            if deviceExist (project, deviceName) then
                printfn $"There is already a device name {deviceName}"
                let device = findDevice (project, deviceName)    
                let plcSoftware = getPlcSoftware device     
                { props with 
                    PlcSoftware = Some plcSoftware
                    Device = Some device }
            else
                let device =
                    project.Devices.CreateWithItem("OrderNumber:" + orderNumber, deviceName, deviceName)
                let plcSoftware = getPlcSoftware device
                printfn "Successfully added device %s" deviceName
                { props with 
                    PlcSoftware = Some plcSoftware
                    Device = Some device }
        | None -> failwithf "Select your project first - use `selectProject`"

    let private tryPlugNew (device: Device, orderNumber: string, hardwareName: string, position: int) =
        try
            device.DeviceItems.[0]
                .PlugNew("OrderNumber:" + orderNumber, hardwareName, position)
        with
        | _ -> 
            printfn "Can't plug new hardware object returning current state"
            device.DeviceItems.[0]

    let plugNew (orderNumber: string, hardwareName: string, position: int) (props: PlcProps) =
        match props.Device with
        | Some device ->
            let deviceItem = tryPlugNew (device, orderNumber, hardwareName, position)

            let deviceItems =
                Array.concat [ props.DeviceItems
                               [| deviceItem |] ]

            { props with DeviceItems = deviceItems }

        | None -> failwithf "Select / Add your device first - use `getDevice`"

    let plugNewHarwareObjects (hardwareObjects: HardwareObject list) (props: PlcProps) =
        match props.Device with
        | Some device ->
            let mutable deviceItems = [||]

            for hardwareObject in hardwareObjects do
                let deviceItem =
                    tryPlugNew (device, hardwareObject.OrderNumber, hardwareObject.Name, hardwareObject.Position)

                deviceItems <-
                    Array.concat [ props.DeviceItems
                                   [| deviceItem |] ]

            { props with DeviceItems = deviceItems }
        | None -> failwithf "Select / Add your device first - use `getDevice`"
    let private tryFindTagTable (plcSoftware: PlcSoftware) tagTableName =
        plcSoftware.TagTableGroup.TagTables
        |> Seq.tryFind (fun tagTable -> tagTable.Name = tagTableName)

    let addTagTable (tagTableName: string) (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            match tryFindTagTable plcSoftware tagTableName with 
            | Some _ ->
                printfn "TagTable %s already existis" tagTableName
                props
            | _ ->
                { props with
                    TagTableList =
                        Array.concat [ props.TagTableList;[| plcSoftware.TagTableGroup.TagTables.Create(tagTableName) |] ] }
        | None -> failwithf "Select / Add your device first - use `getDevice`"

    let private tryFindTag (plcTagComposition: PlcTagComposition) (tag:Tag) =
        plcTagComposition
        |> Seq.tryFind (fun plcTag -> plcTag.Name = tag.Name)

    let private createNewTag (tagTable: PlcTagTable) (tag: Tag) =
        let plcTag = tagTable.Tags.Create(tag.Name)
        plcTag.DataTypeName <- tag.DataType.getName
        plcTag.Comment.Items.[0].Text <- tag.Comment
        plcTag.LogicalAddress <- tag.Address

    let addTags (tags: Tag list, tagTableName) (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            match tryFindTagTable plcSoftware tagTableName with 
            | Some tagTable ->
                for tag in tags do
                    match tryFindTag tagTable.Tags tag with 
                    | Some tag -> printfn "plcTag %s already exists" tag.Name
                    | _ -> createNewTag tagTable tag

                props
            | _ -> failwithf "Can't find selected tagTable, please check the name or first a add new tagTable - use `addTagTable`"   
        | None -> failwithf "Select / Add your device first - use `getDevice`"
    let private tryFindBlockGroup (plcSoftware: PlcSoftware) plcBlockName =
        plcSoftware.BlockGroup.Blocks
        |> Seq.tryFind (fun plcBlock -> plcBlock.Name = plcBlockName)
    let addPlcBlocks plcBlockName (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            match  tryFindBlockGroup plcSoftware plcBlockName with
            | Some plcBlock ->
                printfn "language %A" plcBlock.ProgrammingLanguage
                props
            | None ->
                printfn "PlcBlock %s not found - available blocks" plcBlockName
                for block in  plcSoftware.BlockGroup.Blocks do 
                    printfn "Block %s" block.Name

                props
        | None -> failwithf "Select / Add your device first - use `getDevice`"

    let saveAndClose (props: PlcProps) =
        match props.Project, props.ExistingTiaPortalConnection with
        | Some project, Some tiaPortal ->
            project.Save()
            project.Close()
            tiaPortal.Dispose()
        | _ -> failwithf "Can't save and close because no selected project ; active ExistingTiaPortalConnection %b; active Project %b" props.ExistingTiaPortalConnection.IsSome props.Project.IsSome