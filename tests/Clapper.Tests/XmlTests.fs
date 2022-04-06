module XmlTests
open System.Xml
open System.Xml.Linq
open System.IO
open System.Text
open Expecto
open System
type Xml =
    | Element of string * (string* string) seq * string * Xml seq
    member this.WriteContentTo(writer:XmlWriter) =
        let rec write element =
            match element with
            | Element (name, attributes, value, children) ->
                writer.WriteStartElement(name)
                for a in attributes do
                    writer.WriteAttributeString(fst a, snd a)
                writer.WriteString(value)
                children |> Seq.iter write
                writer.WriteEndElement()
        write this
    override this.ToString() =
        let output = StringBuilder()
        using (new XmlTextWriter(new StringWriter(output),
                Formatting=Formatting.Indented))
            this.WriteContentTo
        output.ToString()

let product productName version =
    Element("Product",[],"",[
        Element("DisplayName",[],productName,[])
        Element("DisplayVersion",[],version,[])
    ])
let optionPackage packageName version=
    Element("OptionPackage",[],"",[
        Element("DisplayName",[],packageName,[])
        Element("DisplayVersion",[],version,[])
    ])

let section name elements =
    Element("Section",[("Name",name)],"",elements)


type UId =
    | UId of int
    member this.Value = (fun (UId id) -> id) this
    member this.ValueAsString = (fun (UId id) -> string id) this

type DataType =
    | Bool
    | Byte
    | Word
    | DWord
    | Int
    | DInt
    | Real
    | S5Time
    | Time
    | Void
    | Struct
    | Custom of string
    member this.GetValue =
        match this with
        | Bool -> "Bool"
        | Byte -> "Byte"
        | Word -> "Word"
        | DWord -> "DWord"
        | Int -> "Int"
        | DInt -> "DInt"
        | Real -> "Real"
        | S5Time -> "S5Time"
        | Time -> "Time"
        | Void -> "Void"
        | Struct -> "Struct"
        | Custom str -> str

let memberElement name (dataType:DataType) accessibility childElements =
    Element("Member",[("Name",name);("Datatype",dataType.GetValue);("Accessibility",accessibility)],"",childElements)

let attributeList =
    Element("AttributeList",[],"",[
        Element("Culture",[],"en-US",[])
        Element("Text",[],"",[])
    ])

let multilingualTextItemElement (id:int) =
    Element("MultilingualTextItem",[("ID",id |> string);("CompositionName","Items")],"",[
        attributeList
    ])
let multilingualTextElement  (id:int)  name =
    Element("MultilingualText",[("ID",id|> string);("CompositionName",name)],"",[
        Element("ObjectList",[],"",[
            multilingualTextItemElement (id+1)
        ])
    ])

type ProgrammingLanguage =
    | LAD
    | FBD
    member this.GetValue =
        match this with
        | LAD -> "LAD"
        | FBD -> "FBD"

type FCBlock = {
    Name : string
    Number : int
    Id : string
    ProgrammingLanguage : ProgrammingLanguage
    Sections : seq<Xml>
    NetworkSource : Xml option
}

type AreaType =
    | Memory
    | Input
    | NoArea
    | DB
    member this.GetValue =
        match this with
        | Memory-> "Memory"
        | Input-> "Input"
        | NoArea -> "None"
        | DB -> "DB"

type Scope =
    | Global
    | Local
    member this.Value =
        match this with
        | Global -> "GlobalVariable"
        | Local -> "LocalVariable"

type Access =
        {
        AreaType : AreaType
        DataType : DataType
        ComponentName : string
        UId : UId
        BitOffset : int option
        BlockNumber : int option
        Scope : Scope
        }
        member this.GetBitOffset =
            match this.BitOffset with
            | Some offset -> offset |> string
            | None -> failwithf "bit offset not set"

type BlockType =
    | FB
    member this.GetValue =
        match this with
        | FB-> "FB"

type Section =
    | Input
    | InOut
    | Output
    member this.GetValue =
        match this with
        | Input-> "Input"
        | InOut-> "InOut"
        | Output-> "Output"


type Call = {
    AreaType : AreaType
    CallInfoName : string
    BlockType : BlockType
    UId : UId
    BitOffset : int
    BlockNumber : int
    InstanceBlockNumber : int
    CreateDate : DateTime
    Parameters : seq<Xml>

}

let accessElement (access : Access) =
    Element("Access",[("Scope",access.Scope.Value);("UId",access.UId.ValueAsString)],"",[
        Element("Symbol",[],"",[
            match access.Scope with
            | Global ->
                Element("Component",[("Name",access.ComponentName)],"",[])
                Element("Address",
                    match access.BlockNumber with
                    | Some bN -> [("Area",access.AreaType.GetValue);("Type",access.DataType.GetValue);("BlockNumber", bN |> string);("BitOffset",access.GetBitOffset);("Informative","true")]
                    | None ->  [("Area",access.AreaType.GetValue);("Type",access.DataType.GetValue);("BitOffset",access.GetBitOffset |> string);("Informative","true")]
                    ,"",[])
            | Local ->
                Element("Component",[("Name",access.ComponentName)],"",[])
        ])
    ])


let parameter name (section :Section) (dataType:DataType) =
    Element("Parameter",[("Name",name);("Section",section.GetValue);("Type",dataType.GetValue)],"",[
                Element("StringAttribute",[("Name","InterfaceFlags");("Informative","true")],"S7_Visible",[])
            ])

let callElement (call : Call) =
    Element("Call",[("UId",call.UId.ValueAsString)],"",[
        Element("CallInfo",[("Name",call.CallInfoName);("BlockType",call.BlockType.GetValue)],"",[
            Element("IntegerAttribute",[("Name","BlockNumber");("Informative","true")],call.BlockNumber |> string,[])
            Element("DateAttribute",[("Name","ParameterModifiedTS");("Informative","true")],call.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss"),[])
            Element("Instance",[("Scope","GlobalVariable");("UId",(call.UId.Value + 1)|> string)],"",[
                Element("Component",[("Name",call.CallInfoName)],"",[])
                Element("Address",[("Area",call.AreaType.GetValue);("Type",call.CallInfoName);("BlockNumber",call.InstanceBlockNumber|> string);("BitOffset",call.BitOffset |> string);("Informative","true")],"",[])

            ])
            for p in call.Parameters do
                p
        ])
    ])

let parts childElements=
    Element("Parts",[],"",childElements)

type WireType =
| IdentCon of UId
| PowerRail
| OpenCon of UId


type Wire = {
    UId : UId
    Name : string
    NameUId : UId
    WireType : WireType
}

let wireElement (wire:Wire)=
    Element("Wire",[("UId", wire.UId.ValueAsString)],"",[
        match wire.WireType with
        | PowerRail ->
            Element("Powerrail",[],"",[])
        | IdentCon uId->
            Element("IdentCon",[("UId",uId.ValueAsString)],"",[])
        | OpenCon uId ->
            Element("OpenCon",[("UId",uId.ValueAsString)],"",[])
        Element("NameCon",[("UId",wire.NameUId.ValueAsString);("Name","en")],"",[])
    ])

let wires childElements=
    Element("Wires",[],"",childElements)

let networkSourceBildungFolgen =
    Element("FlgNet",[("xmlns","http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v2")],"",[
        parts [
            accessElement {
                AreaType = NoArea
                ComponentName = "Folge"
                UId = UId 21
                BitOffset = Some 2392
                BlockNumber = Some 180
                DataType = Bool
                Scope  = Global }
            accessElement
                {
                AreaType = NoArea
                ComponentName = "FrgFolge"
                UId = UId 22
                BitOffset = Some 2360
                BlockNumber = Some 180
                DataType = Bool
                Scope  = Global }
            accessElement
                {
                AreaType = NoArea
                ComponentName = "Temp"
                UId = UId 23
                BitOffset = None
                BlockNumber = None
                DataType = Bool
                Scope  = Local }
            callElement
                {
                    AreaType =  DB
                    CallInfoName = "FB_RobFolge_8"
                    BlockType = FB
                    UId = UId 24
                    BitOffset = 0
                    BlockNumber = 203
                    InstanceBlockNumber = 2086
                    CreateDate = DateTime.Now
                    Parameters=
                        [
                            parameter "PaFeVerk" Input Bool
                            parameter "FrgFolge1" Input Bool
                            parameter "Folge1" Input Bool
                            parameter "FrgFolge2" Input Bool
                            parameter "Folge2" Input Bool
                            parameter "FrgFolge3" Input Bool
                            parameter "Folge3" Input Bool
                            parameter "FrgFolge4" Input Bool
                            parameter "Folge4" Input Bool
                            parameter "FrgFolge5" Input Bool
                            parameter "Folge5" Input Bool
                            parameter "FrgFolge6" Input Bool
                            parameter "Folge6" Input Bool
                            parameter "FrgFolge7" Input Bool
                            parameter "Folge7" Input Bool
                            parameter "FrgFolge8" Input Bool
                            parameter "Folge8" Input Bool
                            parameter "Folge" Output Bool
                            parameter "FrgFolge" Output Bool
                            parameter "PaFe" Output Bool
                            ]
                }
        ]
        wires [
            wireElement {
                    UId = UId 43
                    Name = "en"
                    NameUId = UId 24
                    WireType = PowerRail
                }
            wireElement {
                    UId = UId 44
                    Name = "PaFeVerk"
                    NameUId = UId 24
                    WireType = OpenCon (UId 26)
                }
            wireElement {
                    UId = UId 45
                    Name = "FrgFolge1"
                    NameUId = UId 24
                    WireType = OpenCon (UId 27)
                }
            wireElement {
                    UId = UId 46
                    Name = "Folge1"
                    NameUId = UId 24
                    WireType = OpenCon (UId 28)
                }
            wireElement {
                    UId = UId 47
                    Name = "FrgFolge2"
                    NameUId = UId 24
                    WireType = OpenCon (UId 29)
                }
            wireElement {
                    UId = UId 48
                    Name = "Folge2"
                    NameUId = UId 24
                    WireType = OpenCon (UId 30)
                }
            wireElement {
                    UId = UId 49
                    Name = "FrgFolge3"
                    NameUId = UId 24
                    WireType = OpenCon (UId 31)
                }
            wireElement {
                    UId = UId 50
                    Name = "Folge3"
                    NameUId = UId 24
                    WireType = OpenCon (UId 32)
                }
            wireElement {
                    UId = UId 51
                    Name = "FrgFolge4"
                    NameUId = UId 24
                    WireType = OpenCon (UId 33)
                }
            wireElement {
                    UId = UId 52
                    Name = "Folge4"
                    NameUId = UId 24
                    WireType = OpenCon (UId 34)
                }
            wireElement {
                    UId = UId 53
                    Name = "FrgFolge5"
                    NameUId = UId 24
                    WireType = OpenCon (UId 35)
                }
            wireElement {
                    UId = UId 54
                    Name = "Folge5"
                    NameUId = UId 24
                    WireType = OpenCon (UId 36)
                }
            wireElement {
                    UId = UId 55
                    Name = "FrgFolge6"
                    NameUId = UId 24
                    WireType = OpenCon (UId 37)
                }
            wireElement {
                    UId = UId 56
                    Name = "Folge6"
                    NameUId = UId 24
                    WireType = OpenCon (UId 38)
                }
            wireElement {
                    UId = UId 57
                    Name = "FrgFolge7"
                    NameUId = UId 24
                    WireType = OpenCon (UId 39)
                }
            wireElement {
                    UId = UId 58
                    Name = "Folge7"
                    NameUId = UId 24
                    WireType = OpenCon (UId 40)
                }
            wireElement {
                    UId = UId 59
                    Name = "FrgFolge8"
                    NameUId = UId 24
                    WireType = OpenCon (UId 41)
                }
            wireElement {
                    UId = UId 60
                    Name = "Folge8"
                    NameUId = UId 24
                    WireType = OpenCon (UId 42)
                }
            wireElement {
                    UId = UId 61
                    Name = "Folge"
                    NameUId = UId 24
                    WireType = IdentCon (UId 21)
                }
            wireElement {
                    UId = UId 62
                    Name = "FrgFolge"
                    NameUId = UId 24
                    WireType = IdentCon (UId 22)
                }
            wireElement {
                    UId = UId 63
                    Name = "PaFe"
                    NameUId = UId 24
                    WireType = IdentCon (UId 23)
                }
        ]
    ])
let networkSourceFrgFolge =
    Element("FlgNet",[("xmlns","http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v2")],"",[
        parts [
            accessElement {
                AreaType = NoArea
                ComponentName = "Folge"
                UId = UId 21
                BitOffset = Some 2392
                BlockNumber = Some 180
                DataType = Byte
                Scope  = Global}
            accessElement
                {
                AreaType = NoArea
                ComponentName = "FrgFolge"
                UId = UId 22
                BitOffset = Some 2360
                BlockNumber = Some 180
                DataType = Bool
                Scope  = Global }
            accessElement
                {
                AreaType = NoArea
                ComponentName = "Temp"
                UId = UId 23
                BitOffset = Some 2360
                BlockNumber = None
                DataType = Bool
                Scope  = Local }
            callElement
                {
                    AreaType =  DB
                    CallInfoName = "FB_Rob_PN_A"
                    BlockType = FB
                    UId = UId  23
                    BitOffset = 0
                    BlockNumber = 201
                    InstanceBlockNumber = 2086
                    CreateDate = DateTime.Now
                    Parameters=
                        [
                            parameter "Rob_A" Input (Custom "Pointer")
                            parameter "ST_ROB" Input (Custom"ST_Rob")]
                }
        ]
        wires [
            wireElement {
                    UId = UId 25
                    Name = "en"
                    NameUId = UId 23
                    WireType = PowerRail
                }
            wireElement {
                    UId = UId 26
                    Name = "Rob_A"
                    NameUId = UId 23
                    WireType = IdentCon (UId 21)
                }
            wireElement {
                    UId = UId 27
                    Name = "ST_ROB"
                    NameUId = UId 23
                    WireType = IdentCon (UId 22)
                }
        ]
    ])


let blockCompileUnit (fcBlock:FCBlock)=
    Element("SW.Blocks.CompileUnit",[("ID",fcBlock.Id);("CompositionName","CompileUnits")],"",[
        Element("AttributeList",[],"",[
            match fcBlock.NetworkSource with
            | Some networkSource -> Element("NetworkSource",[],"",[networkSource])
            | None -> Element("NetworkSource",[],"",[])
            Element("ProgrammingLanguage",[],fcBlock.ProgrammingLanguage.GetValue,[])

        ])
        Element("ObjectList",[],"",[
            multilingualTextElement 4 "Comment"
            multilingualTextElement 6 "Title"
        ])
    ])


let sectionsRobo =
    [
        section "Input" []
        section "Output" []
        section "InOut" []
        section "Temp" [
            memberElement "Temp" Struct "Public" [
                memberElement "_Bool" Bool "Public" []
                memberElement "_Byte" Byte "Public" []
                memberElement "_Word" Word "Public" []
                memberElement "_DWord" DWord "Public" []
                memberElement "_Int" Int "Public" []
                memberElement "_DInt" DInt "Public" []
                memberElement "_Real" Real "Public" []
                memberElement "_S5Time" S5Time "Public" []
                memberElement "_Time" Time "Public" []
            ]
            ]
        section "Constant" []
        section "Return" [
            memberElement "Ret_Val" Void "Public" []
        ]]
let sectionsInput1 =
    [
        section "Input" [
            memberElement "input0" Bool "Public" []
            memberElement "input1" Bool "Public" []
            memberElement "input2" Bool "Public" []
            memberElement "input3" Bool "Public" []
        ]
        section "Output" [
            memberElement "output0" Bool "Public" []
            memberElement "output1" Bool "Public" []
            memberElement "output2" Bool "Public" []
            memberElement "output3" Bool "Public" []
        ]
        section "InOut" []
        section "Temp" []
        section "Constant" []
        section "Return" [
            memberElement "Ret_Val" Void "Public" []
        ]]


let buildFcBlock (block:FCBlock) =
    Element("SW.Blocks.FC",[("ID","0")],"",[
        Element("AttributeList",[],"",[
            Element("AutoNumber",[],"true",[])
            Element("HeaderAuthor",[],"",[])
            Element("HeaderFamily",[],"",[])
            Element("HeaderName",[],"",[])
            Element("HeaderVersion",[],"1.1",[])
            Element("Interface",[],"",[
                Element("Sections",[("xmlns","http://www.siemens.com/automation/Openness/SW/Interface/v5")],"",block.Sections
                )
            ])
            Element("IsIECCheckEnabled",[],"false",[])
            Element("MemoryLayout",[],"Optimized",[])
            Element("Name",[],block.Name,[])
            Element("Number",[],block.Number |> string,[])
            Element("ProgrammingLanguage",[],block.ProgrammingLanguage.GetValue,[])
            Element("SetENOAutomatically",[],"false",[])
            Element("StructureModified",[("ReadOnly","true")],"2017-06-20T14:56:00.5916118Z",[])
            Element("UDABlockProperties",[],"",[])
            Element("UDAEnableTagReadback",[],"false",[])

        ])
        Element("ObjectList",[],"",[
            multilingualTextElement 1 "Comment"
            blockCompileUnit block
            multilingualTextElement 8 "Title"
        ])
    ])


let documentInfo version block =
    Element("Document",[],"",[
            Element("Engineering",[("version",version)],"",[])
            Element("DocumentInfo",[],"",[
                Element("Created",[],"2022-03-11T11:27:36.1676076Z",[])
                Element("ExportSetting",[],"WithDefaults",[])
                Element("InstalledProducts",[],"",[
                    product "Totally Integrated Automation Portal" version
                    optionPackage "TIA Portal Openness" version
                    optionPackage "TIA Portal Version Control Interface" version
                    product "STEP 7 Professional" version
                    optionPackage "STEP 7 Safety" version
                    product "WinCC Advanced" version
                ])
            ])
            block
        ])
let createBlock (name,version,block) =
    let doc = XDocument(XElement.Parse((documentInfo version block).ToString()))
    doc.Save(Path.GetFullPath($"./TestFolder/{name}.xml"))
    doc.ToString()

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
        test "BildungFolgen" {
                let fcBlock  ={
                    Name = "ROBNAME"
                    Number  = 71
                    Id = "E"
                    ProgrammingLanguage = LAD
                    Sections = sectionsRobo
                    NetworkSource = Some networkSourceBildungFolgen
                }
                let actual = createBlock ("BildungFolgen","V17",buildFcBlock fcBlock)
                let expected = Path.GetFullPath("templates/BildungFolgen.xml") |> File.ReadAllText
                Expect.equal actual expected "BildungFolgen file should match"
            }

    ]
let result = runTests defaultConfig (tests())
printfn "result %A" result