open Clapper
open System.IO
open XmlHelper
open Block   
open PlcDataType
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

let sectionsRobo =
    [
        Section.section Section.Input []
        Section.section Section.Output []
        Section.section Section.InOut []
        Section.section Section.Temp [
            Section.memberElement "Temp" Struct Section.Public [
                Section.memberElement "_Bool" Bool Section.Public []
                Section.memberElement "_Byte" Byte Section.Public []
                Section.memberElement "_Word" Word Section.Public []
                Section.memberElement "_DWord" DWord Section.Public []
                Section.memberElement "_Int" Int Section.Public []
                Section.memberElement "_DInt" DInt Section.Public []
                Section.memberElement "_Real" Real Section.Public []
                Section.memberElement "_S5Time" S5Time Section.Public []
                Section.memberElement "_Time" Time Section.Public []
            ]
            ]
        Section.section Section.Constant []
        Section.section Section.Return [  
           Section.memberElement "Ret_Val" Void Section.Public []
        ]]

let networkSourceTypRoboter blockName =

    Element("FlgNet",[("xmlns","http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v2")],"",[
        NetworkSource.parts [
            NetworkSource.accessElement {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 21
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 22
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 23
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 24
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 25
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 26
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 27
                BitOffset = Some 2360
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.Memory
                ComponentName = "INTROSYS_VKE=0"
                UId = UId 28
                BitOffset = Some 80
                BlockNumber = None
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.NoArea
                ComponentName = "E233_240_Typinfo_SPS"
                UId = UId 29
                BitOffset = Some 912
                BlockNumber = Some 180
                DataType = Byte
                Scope  = Global }
            NetworkSource.callElement
                {
                    BlockName = blockName
                    AreaType =  NetworkSource.DB
                    CallInfoName = "BIT_TO_BYTE"
                    BlockType = NetworkSource.FC
                    UId = UId 30
                    BitOffset = 0
                    BlockNumber = 679
                    InstanceBlockNumber = 2087
                    CreateDate = System.DateTime.Now
                    Parameters=
                        [
                            NetworkSource.parameter "BIT_0" Section.Input Bool
                            NetworkSource.parameter "BIT_1" Section.Input Bool
                            NetworkSource.parameter "BIT_2" Section.Input Bool
                            NetworkSource.parameter "BIT_3" Section.Input Bool
                            NetworkSource.parameter "BIT_4" Section.Input Bool
                            NetworkSource.parameter "BIT_5" Section.Input Bool
                            NetworkSource.parameter "BIT_6" Section.Input Bool
                            NetworkSource.parameter "BIT_7" Section.Input Bool
                            NetworkSource.parameter "Ret_Val" Section.Return Byte
                            ]
                }
        ]
        Wires.wires [
            Wires.wireElement {
                    UId = UId 31
                    Name = "en"
                    NameUId = UId 30
                    WireType = Wires.PowerRail
                }
            Wires.wireElement {
                    UId = UId 32
                    Name = "BIT_0"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 21)
                }
            Wires.wireElement {
                    UId = UId 33
                    Name = "BIT_1"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 22)
                }
            Wires.wireElement {
                    UId = UId 34
                    Name = "BIT_2"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 23)
                }
            Wires.wireElement {
                    UId = UId 35
                    Name = "BIT_3"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 24)
                }
            Wires.wireElement {
                    UId = UId 36
                    Name = "BIT_4"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 25)
                }
            Wires.wireElement {
                    UId = UId 37
                    Name = "BIT_5"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 26)
                }
            Wires.wireElement {
                    UId = UId 38
                    Name = "BIT_6"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 27)
                }
            Wires.wireElement {
                    UId = UId 39
                    Name = "BIT_7"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 28)
                }
            Wires.wireElement {
                    UId = UId 40
                    Name = "Ret_Val"
                    NameUId = UId 30
                    WireType = Wires.IdentCon (UId 29)
                }
        ]
    ])

let fcBlock  ={
        Name = "ROBNAME"
        Number  = 71
        FCBlockId = FCBlockId 0
        CompileUnitId = CompileUnitId "E"
        ProgrammingLanguage = LAD
        Sections = sectionsRobo
        NetworkSource = Some (networkSourceTypRoboter "ROBNAME")
        CreateTime = System.DateTime.Now
    }


let sectionsFREQ_COUNTER =
    [ Section.section
          Section.NoSection
          [ Section.memberElement
                "litrsPerPuls"
                UInt
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ]
            Section.memberElement
                "prewHour"
                UInt
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ]

            Section.memberElement
                "curHour"
                UInt
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ]
            Section.memberElement
                "prewDay"
                UDInt
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ]
            Section.memberElement
                "curDay"
                UDInt
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ]
            Section.memberElement
                "all"
                UDInt
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ]
            Section.memberElement
                "rst"
                Bool
                Section.NoAccessibility
                [ attributeElement [ dataTypeAttribute Bool ExternalAccessible
                                     dataTypeAttribute Bool ExternalVisible
                                     dataTypeAttribute Bool ExternalWritable
                                     dataTypeAttribute Bool SetPoint ] ] ] ]

let plcDataType: PlcDataType =
    {   Name = "FREQ_COUNTER"
        Number = 71   
        DataTypeId = DataTypeId 0
        Sections = sectionsFREQ_COUNTER
        CreationTime = System.DateTime.Now }

@"C:\Users\TimForkmann\Documents\Automatisierung\"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "ESA Kuwait"
|> PlcProgram.addAllLanguages
|> PlcProgram.getDevice ("6ES7 510-1DJ01-0AB0/V2.9","ET200SP")
|> PlcProgram.plugNewHarwareObjects hardwareObjects
|> PlcProgram.addTagTable tagTableList
|> PlcProgram.addTags (tags,tagTableList)
// |> PlcProgram.importPlcType (Path.GetFullPath "./xmlimport/datatypes/FREQ_COUNTER.xml")
// |> PlcProgram.importPlcType (Path.GetFullPath "./xmlimport/datatypes/PropDosing.xml")
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./xmlimport/programblocks/dbVolumeCounter.xml")
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./xmlimport/programblocks/VolumeCounter.xml")
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./xmlimport/programblocks/AllPump.xml")
|> PlcProgram.createAndImportPlcType ("FREQ_COUNTER",V17,plcDataType)
|> PlcProgram.createAndImportBlock ("TypRoboter",V17,FunctionalBlock fcBlock)  
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/BildungFolgen.xml")
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/TypRoboter.xml")
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/Main.xml")
// // |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/dbVolumeCounter.xml")
// // |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/VolumeCounter.xml")
// // |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/Motor.xml")
// |> PlcProgram.importPlcBlock (Path.GetFullPath "./testFolder/FB1.xml")
// |> PlcProgram.exportPlcBlock "Main"

|> PlcProgram.saveAndClose


