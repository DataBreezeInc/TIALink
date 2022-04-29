namespace Clapper

open System
open System.IO
open Siemens.Engineering
open Siemens.Engineering.HW
open Siemens.Engineering.HW.Features
open Siemens.Engineering.SW
open Siemens.Engineering.SW.Tags
open Siemens.Engineering.SW.Blocks
open Siemens.Engineering.SW.Types
open Siemens.Engineering.Compiler
open System.Globalization
open XmlHelper
open System.Xml.Linq

type HardwareObject =
    { OrderNumber: string
      Name: string
      Position: int }

type Tag =
    { Name: string
      DataType: DataType
      Comment: string
      Address: string }




type Block =
    { Name: string
      IsAutoNumbered: bool
      Number: int
      BlockType: BlockType }

[<RequireQualifiedAccess>]
module PlcProgram =
    type PlcProps =
        private
            { ExistingTiaPortalConnection: TiaPortal option
              Project: Project option
              Device: Device option
              PlcSoftware: PlcSoftware option
              PlcBlock: PlcBlock option
              ProjectName: string
              UserInterface: bool
              ProjectPath: string
              DeviceItems: DeviceItem []
              TagTableList: PlcTagTable []
              PlcTypeList: PlcType []
              Compiled : bool }

    let private defaultProps () =
        { ExistingTiaPortalConnection = None
          Project = None
          Device = None
          PlcSoftware = None
          PlcBlock = None
          UserInterface = false
          ProjectPath = ""
          ProjectName = ""
          DeviceItems = [||]
          TagTableList = [||]
          PlcTypeList = [||]
          Compiled = false }

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

    let addLanguage (language: Language) (props: PlcProps) =
        match props.Project with
        | Some project ->
            let languageSettings = project.LanguageSettings
            let supportedLanguages = languageSettings.Languages
            let activeLanguages = languageSettings.ActiveLanguages

            let language =
                supportedLanguages.Find(CultureInfo.GetCultureInfo(language.Value))

            activeLanguages.Add(language)
            props
        | None -> failwithf "Select your project first - use `selectProject`"

    let addAllLanguages (props: PlcProps) =
        match props.Project with
        | Some project ->
            let languageSettings = project.LanguageSettings
            let supportedLanguages = languageSettings.Languages

            for supportedLanguage in supportedLanguages do
                match languageSettings.ActiveLanguages
                      |> Seq.tryFind (fun l -> l.Culture = supportedLanguage.Culture)
                    with
                | Some _ -> ()
                | None -> languageSettings.ActiveLanguages.Add(supportedLanguage)

            props
        | None -> failwithf "Select your project first - use `selectProject`"


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

    let plugNew (hardwareObject: HardwareObject) (props: PlcProps) =
        match props.Device with
        | Some device ->
            let deviceItem = tryPlugNew (device, hardwareObject.OrderNumber, hardwareObject.Name, hardwareObject.Position)

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
                        Array.concat [ props.TagTableList
                                       [| plcSoftware.TagTableGroup.TagTables.Create(tagTableName) |] ] }
        | None -> failwithf "Select / Add your device first - use `getDevice`"

    let private tryFindTag (plcTagComposition: PlcTagComposition) (tag: Tag) =
        plcTagComposition
        |> Seq.tryFind (fun plcTag -> plcTag.Name = tag.Name)

    let private createNewTag (tagTable: PlcTagTable) (tag: Tag) =
        let plcTag = tagTable.Tags.Create(tag.Name)
        plcTag.DataTypeName <- tag.DataType.Value
        plcTag.Comment.Items.[0].Text <- tag.Comment
        plcTag.LogicalAddress <- tag.Address

    let addTag (tag: Tag, tagTableName) (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            match tryFindTagTable plcSoftware tagTableName with
            | Some tagTable ->
                match tryFindTag tagTable.Tags tag with
                | Some tag -> printfn "plcTag %s already exists" tag.Name
                | _ -> createNewTag tagTable tag

                props
            | _ ->
                failwithf
                    "Can't find selected tagTable, please check the name or first a add new tagTable - use `addTagTable`"
        | None -> failwithf "Select / Add your device first - use `getDevice`"

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
            | _ ->
                failwithf
                    "Can't find selected tagTable, please check the name or first a add new tagTable - use `addTagTable`"
        | None -> failwithf "Select / Add your device first - use `getDevice`"

    let private tryFindPlcType (plcSoftware: PlcSoftware) plcTypeName =
        plcSoftware.TypeGroup.Types
        |> Seq.tryFind (fun plcType -> plcType.Name = plcTypeName)

    let importExportFolder (name: string) =
        let targetDir =
            if not (Directory.Exists("./tiaWorkDir")) then
                Directory.CreateDirectory("./tiaWorkDir")
                |> ignore

                "./tiaWorkDir"
            else
                "./tiaWorkDir"

        Path.GetFullPath($"{targetDir}/{name}.xml")

    let importPlcType (plcTypeName: string) (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            try
                plcSoftware.TypeGroup.Types.Import((FileInfo(importExportFolder plcTypeName)), ImportOptions.Override)
                |> ignore

                printfn "Imported %s" plcTypeName
                props
            with
            | exn -> failwithf "Could not import PlcType %A" exn.Message

        | _ -> failwithf "Select / Add your device first - use `getDevice`"


    let createPlcType (name, version, plcDataType) =
        let doc =
            XDocument(
                XElement.Parse(
                    (PlcDataType.documentInfo version plcDataType)
                        .ToString()
                )
            )

        doc.Save(importExportFolder name)

        [ """<?xml version="1.0" encoding="utf-8"?>"""
          Environment.NewLine
          doc.ToString() ]
        |> String.concat ""

    let createAndImportPlcType (name, version: TiaVersion, plcDataType) (props: PlcProps) =
        try
            let _ = createPlcType (name, version, plcDataType)
            printfn "Created PLC Types %s" name
            importPlcType name props
        with
        | exn -> failwithf "Could not create PlcBlock %A" exn.Message


    let private tryFindBlockGroup (plcSoftware: PlcSoftware) plcBlockName =
        plcSoftware.BlockGroup.Blocks
        |> Seq.tryFind (fun plcBlock ->
            printfn "blockName %s" plcBlock.Name
            plcBlock.Name = plcBlockName)

    let exportPlcBlock (blockName: string) (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            if props.Compiled then
                match tryFindBlockGroup plcSoftware blockName with
                | Some plcBlock ->
                    plcBlock.Export(FileInfo(props.ProjectPath + blockName + ".xlm"), ExportOptions.WithDefaults)
                    props
                | None ->
                    failwithf "Could not find block %s" blockName
                    props
            else  
                failwithf "You have to compile your project first before you can export your blocks - use `compileProject`"      
        | _ -> failwithf "Select / Add your device first - use `getDevice`"
    let exportAllPlcBlocks (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            for plcBlock in plcSoftware.BlockGroup.Blocks do
            
                plcBlock.Export(FileInfo(Path.GetFullPath($"{props.ProjectPath}/{props.ProjectName}/Exports/{plcBlock.Name}.xml")), ExportOptions.WithDefaults)
                printfn "Successfully exported PlcBlock %s" plcBlock.Name
            props
        | _ -> failwithf "Select / Add your device first - use `getDevice`"


    let importPlcBlock name (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            try
                plcSoftware.BlockGroup.Blocks.Import((FileInfo(importExportFolder name)), ImportOptions.Override)
                |> ignore

                printfn "Imported %s" name
                props
            with
            | exn -> failwithf "Could not import PlcBlock %A" exn.Message

        | _ -> failwithf "Select / Add your device first - use `getDevice`"

    let createAndExportBlock (name, version: TiaVersion, block) =
        let doc = XDocument(XElement.Parse((Block.documentInfo version block).ToString()))
        doc.Save(importExportFolder name)
        [ """<?xml version="1.0" encoding="utf-8"?>"""
          Environment.NewLine
          doc.ToString() ]
        |> String.concat ""

    let createAndImportBlock (name, version: TiaVersion, (block: Block.BlockType)) (props: PlcProps) =
        try
            let _ = createAndExportBlock (name, version, block)
            printfn "Created FCBlock %s" name
            importPlcBlock name props
        with
        | exn -> failwithf "Could not create PlcBlock %A" exn.Message
    let createDataBlock (dataBlock:Block.GlobalDB) (props: PlcProps) =
        try
            let _ = createAndExportBlock (dataBlock.Name, dataBlock.TiaVersion, Block.GlobalDB dataBlock)
            printfn "Created DataBlock %s" dataBlock.Name
            importPlcBlock dataBlock.Name props
        with
        | exn -> failwithf "Could not create PlcBlock %A" exn.Message
    let createFunctionalBlock (functionalBlock:Block.FCBlock) (props: PlcProps) =
        try
            let _ = createAndExportBlock (functionalBlock.Name, functionalBlock.TiaVersion, Block.FunctionalBlock functionalBlock)
            printfn "Created FunctionalBlock %s" functionalBlock.Name
            importPlcBlock functionalBlock.Name props
        with
        | exn -> failwithf "Could not create PlcBlock %A" exn.Message

    let compileProject (props: PlcProps) =
        match props.PlcSoftware with
        | Some plcSoftware ->
            let compileService = plcSoftware.GetService<ICompilable>()
            let result = compileService.Compile()  
            printfn 
                $"Result :  {result.State.ToString()} 
                Errors: {result.ErrorCount.ToString()} 
                Warnings: {result.WarningCount.ToString()} 
                Compiler"
            { props with Compiled = true}
        | _ -> failwithf "Select / Add your device first - use `getDevice`"

    let saveAndClose (props: PlcProps) =
        match props.Project, props.ExistingTiaPortalConnection with
        | Some project, Some tiaPortal ->
            project.Save()
            project.Close()
            tiaPortal.Dispose()
        | _ ->
            failwithf
                "Can't save and close because no selected project ; active ExistingTiaPortalConnection %b; active Project %b"
                props.ExistingTiaPortalConnection.IsSome
                props.Project.IsSome