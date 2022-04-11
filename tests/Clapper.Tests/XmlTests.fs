module XmlTests

open System.IO
open Expecto
open System
open XmlHelper
open Clapper

let networkSourceBildungFolgen blockName =
    Element("FlgNet",[("xmlns","http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v2")],"",[
        NetworkSource.parts [
            NetworkSource.accessElement {
                AreaType = NetworkSource.NoArea
                ComponentName = "Folge"
                UId = UId 21
                BitOffset = Some 2392
                BlockNumber = Some 180
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.NoArea
                ComponentName = "FrgFolge"
                UId = UId 22
                BitOffset = Some 2360
                BlockNumber = Some 180
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.NoArea
                ComponentName = "Temp"
                UId = UId 23
                BitOffset = None
                BlockNumber = None
                DataType = Bool
                Scope  = Local }
            NetworkSource.callElement
                {
                    BlockName = blockName
                    AreaType =  NetworkSource.DB
                    CallInfoName = "FB_RobFolge_8"
                    BlockType = NetworkSource.FB
                    UId = UId 24
                    BitOffset = 0
                    BlockNumber = 203
                    InstanceBlockNumber = 2087
                    CreateDate = DateTime.Now
                    Parameters=
                        [
                            NetworkSource.parameter "PaFeVerk" Section.Input Bool
                            NetworkSource.parameter "FrgFolge1" Section.Input Bool
                            NetworkSource.parameter "Folge1" Section.Input Byte
                            NetworkSource.parameter "FrgFolge2" Section.Input Bool
                            NetworkSource.parameter "Folge2" Section.Input Byte
                            NetworkSource.parameter "FrgFolge3" Section.Input Bool
                            NetworkSource.parameter "Folge3" Section.Input Byte
                            NetworkSource.parameter "FrgFolge4" Section.Input Bool
                            NetworkSource.parameter "Folge4" Section.Input Byte
                            NetworkSource.parameter "FrgFolge5" Section.Input Bool
                            NetworkSource.parameter "Folge5" Section.Input Byte
                            NetworkSource.parameter "FrgFolge6" Section.Input Bool
                            NetworkSource.parameter "Folge6" Section.Input Byte
                            NetworkSource.parameter "FrgFolge7" Section.Input Bool
                            NetworkSource.parameter "Folge7" Section.Input Byte
                            NetworkSource.parameter "FrgFolge8" Section.Input Bool
                            NetworkSource.parameter "Folge8" Section.Input Byte
                            NetworkSource.parameter "Folge" Section.Output Byte
                            NetworkSource.parameter "FrgFolge" Section.Output Bool
                            NetworkSource.parameter "PaFe" Section.Output Bool
                            ]
                }
        ]
        Wires.wires [
            Wires.wireElement {
                    UId = UId 43
                    Name = "en"
                    NameUId = UId 24
                    WireType = Wires.PowerRail
                }
            Wires.wireElement {
                    UId = UId 44
                    Name = "PaFeVerk"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 26)
                }
            Wires.wireElement {
                    UId = UId 45
                    Name = "FrgFolge1"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 27)
                }
            Wires.wireElement {
                    UId = UId 46
                    Name = "Folge1"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 28)
                }
            Wires.wireElement {
                    UId = UId 47
                    Name = "FrgFolge2"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 29)
                }
            Wires.wireElement {
                    UId = UId 48
                    Name = "Folge2"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 30)
                }
            Wires.wireElement {
                    UId = UId 49
                    Name = "FrgFolge3"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 31)
                }
            Wires.wireElement {
                    UId = UId 50
                    Name = "Folge3"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 32)
                }
            Wires.wireElement {
                    UId = UId 51
                    Name = "FrgFolge4"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 33)
                }
            Wires.wireElement {
                    UId = UId 52
                    Name = "Folge4"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 34)
                }
            Wires.wireElement {
                    UId = UId 53
                    Name = "FrgFolge5"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 35)
                }
            Wires.wireElement {
                    UId = UId 54
                    Name = "Folge5"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 36)
                }
            Wires.wireElement {
                    UId = UId 55
                    Name = "FrgFolge6"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 37)
                }
            Wires.wireElement {
                    UId = UId 56
                    Name = "Folge6"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 38)
                }
            Wires.wireElement {
                    UId = UId 57
                    Name = "FrgFolge7"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 39)
                }
            Wires.wireElement {
                    UId = UId 58
                    Name = "Folge7"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 40)
                }
            Wires.wireElement {
                    UId = UId 59
                    Name = "FrgFolge8"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 41)
                }
            Wires.wireElement {
                    UId = UId 60
                    Name = "Folge8"
                    NameUId = UId 24
                    WireType = Wires.OpenCon (UId 42)
                }
            Wires.wireElement {
                    UId = UId 61
                    Name = "Folge"
                    NameUId = UId 24
                    WireType = Wires.IdentCon (UId 21)
                }
            Wires.wireElement {
                    UId = UId 62
                    Name = "FrgFolge"
                    NameUId = UId 24
                    WireType = Wires.IdentCon (UId 22)
                }
            Wires.wireElement {
                    UId = UId 63
                    Name = "PaFe"
                    NameUId = UId 24
                    WireType = Wires.IdentCon (UId 23)
                }
        ]
    ])

let networkSourceFrgFolge blockName =
    Element("FlgNet",[("xmlns","http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v2")],"",[
        NetworkSource.parts [
            NetworkSource.accessElement {
                AreaType = NetworkSource.NoArea
                ComponentName = "Folge"
                UId = UId 21
                BitOffset = Some 2392
                BlockNumber = Some 180
                DataType = Byte
                Scope  = Global}
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.NoArea
                ComponentName = "FrgFolge"
                UId = UId 22
                BitOffset = Some 2360
                BlockNumber = Some 180
                DataType = Bool
                Scope  = Global }
            NetworkSource.accessElement
                {
                AreaType = NetworkSource.NoArea
                ComponentName = "Temp"
                UId = UId 23
                BitOffset = Some 2360
                BlockNumber = None
                DataType = Bool
                Scope  = Local }
            NetworkSource.callElement
                {
                    BlockName = blockName
                    AreaType =  NetworkSource.DB
                    CallInfoName = "FB_Rob_PN_A"
                    BlockType = NetworkSource.FB
                    UId = UId  23
                    BitOffset = 0
                    BlockNumber = 201
                    InstanceBlockNumber = 2086
                    CreateDate = DateTime.Now
                    Parameters=
                        [
                            NetworkSource.parameter "Rob_A" Section.Input (Custom "Pointer")
                            NetworkSource.parameter "ST_ROB" Section.Input (Custom"ST_Rob")]
                }
        ]
        Wires.wires [
            Wires.wireElement {
                    UId = UId 25
                    Name = "en"
                    NameUId = UId 23
                    WireType = Wires.PowerRail
                }
            Wires.wireElement {
                    UId = UId 26
                    Name = "Rob_A"
                    NameUId = UId 23
                    WireType = Wires.IdentCon (UId 21)
                }
            Wires.wireElement {
                    UId = UId 27
                    Name = "ST_ROB"
                    NameUId = UId 23
                    WireType = Wires.IdentCon (UId 22)
                }
        ]
    ])



let sectionsRobo =
    [
        Section.section Section.Input []
        Section.section Section.Output []
        Section.section Section.InOut []
        Section.section Section.Temp [
            Section.memberElement "Temp" Struct "Public" [
                Section.memberElement "_Bool" Bool "Public" []
                Section.memberElement "_Byte" Byte "Public" []
                Section.memberElement "_Word" Word "Public" []
                Section.memberElement "_DWord" DWord "Public" []
                Section.memberElement "_Int" Int "Public" []
                Section.memberElement "_DInt" DInt "Public" []
                Section.memberElement "_Real" Real "Public" []
                Section.memberElement "_S5Time" S5Time "Public" []
                Section.memberElement "_Time" Time "Public" []
            ]
            ]
        Section.section Section.Constant []
        Section.section Section.Return [
           Section.memberElement "Ret_Val" Void "Public" []
        ]]

let sectionsInput1 =
    [
        Section.section Section.Input [
            Section.memberElement "input0" Bool "Public" []
            Section.memberElement "input1" Bool "Public" []
            Section.memberElement "input2" Bool "Public" []
            Section.memberElement "input3" Bool "Public" []
        ]
        Section.section Section.Output [
            Section.memberElement "output0" Bool "Public" []
            Section.memberElement "output1" Bool "Public" []
            Section.memberElement "output2" Bool "Public" []
            Section.memberElement "output3" Bool "Public" []
        ]
        Section.section Section.InOut []
        Section.section Section.Temp []
        Section.section Section.Constant []
        Section.section Section.Return [
            Section.memberElement "Ret_Val" Void "Public" []
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

let tests () =
    testList "XmlParser" [
        // test "Input_1" {
        //         let actual = createBlock ("Input_1","V17",fcBlock "Input_1")
        //         let expected = Path.GetFullPath("templates/Inputs_1.xml") |> File.ReadAllText
        //         Expect.equal actual expected "Input file should match"
        //     }
        // test "EingabenLesen" {
        //         let fcBlock  ={
        //             Name = "ROBNAME"
        //             Number  = 71
        //             Id = "3"
        //             ProgrammingLanguage = LAD
        //             Sections = sectionsRobo
        //             NetworkSource = Some networkSourceInputRead
        //         }
        //         let actual = createBlock ("EingabenLesen","V17",buildFcBlock fcBlock)
        //         let expected = Path.GetFullPath("templates/EingabenLesen.xml") |> File.ReadAllText
        //         Expect.equal actual expected "EingabenLesen file should match"
        //     }
        // test "BildungFolgen" {
        //         let fcBlock  ={
        //             Name = "ROBNAME"
        //             Number  = 71
        //             Id = "E"
        //             ProgrammingLanguage = LAD
        //             Sections = sectionsRobo
        //             NetworkSource = Some (networkSourceBildungFolgen "ROBNAME")
        //         }
        //         let actual = createBlock ("BildungFolgen","V17",buildFcBlock fcBlock)
        //         let expected = Path.GetFullPath("templates/BildungFolgen.xml") |> File.ReadAllText
        //         Expect.equal actual expected "BildungFolgen file should match"
        //     }
        test "TypeRoboter" {
                let fcBlock  ={
                    Name = "ROBNAME"
                    Number  = 71
                    FCBlockId = FCBlockId 0
                    CompileUnitId = CompileUnitId "E"
                    ProgrammingLanguage = LAD
                    Sections = sectionsRobo
                    NetworkSource = Some (networkSourceTypRoboter "ROBNAME")
                }
                let actual = PlcProgram.createAndExportBlock ("TypRoboter",V17,FunctionalBlock fcBlock,"testFolder")
                let expected = Path.GetFullPath("templates/TypRoboter.xml") |> File.ReadAllText
                Expect.equal actual expected "TypRoboter file should match"
            }

    ]
let result = runTests defaultConfig (tests())
printfn "result %A" result