module XmlTests
open System.Xml
open System.Xml.Linq
open System.IO
open System.Text
open Expecto

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
    ProgrammingLanguage : ProgrammingLanguage
    Sections : seq<Xml>
}

let networkSource =
    Element("FlgNet",[("xmlns","http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v2")],"",[
        Element("Parts",[],"",[
            Element("Access",[("Scope","GlobalVariable");("UId","21")],"",[
                Element("Symbol",[],"",[
                    Element("Component",[("Name","ROBNAMEA001RFLGW1")],"",[])
                    Element("Address",[("Area","Input");("Type","Bool");("BitOffset","8192");("Informative","true")],"",[])
                ])
            ])
            Element("Access",[("Scope","GlobalVariable");("UId","22")],"",[
                Element("Symbol",[],"",[
                    Element("Component",[("Name","Rob")],"",[])
                    Element("Address",[("Area","None");("Type","ST_Rob");("BlockNummer","180");("BitOffset","0")],"",[])
                ])
            ])
            Element("Call",[("UId","23")],"",[
                Element("CallInfo",[("Name","FB_Rob_PN_A");("BlockType","FB")],"201",[
                    Element("IntegerAttribute",[("Name","BlockNumber");("Informative","true")],"2017-06-08T09:11:58",[])
                    Element("DateAttribute",[("Name","ParameterModifiedTS");("Informative","true")],"",[])
                    Element("Instance",[("Scope","GlobalVariable");("UId","24")],"",[
                        Element("Component",[("Name","ROBNAME#FB_Rob_PN_A_DB")],"",[])
                        Element("Address",[("Area","DB");("Type","FB_Rob_PN_A");("BlockNumber","2086");("BitOffset","0");("Informative","true")],"",[])

                    ])
                    Element("Address",[("Area","None");("Type","ST_Rob");("BlockNumber","180");("BitOffset","0")],"",[])
                    Element("Component",[("Name","Rob")],"",[])
                    Element("Address",[("Area","None");("Type","ST_Rob");("BlockNumber","180");("BitOffset","0")],"",[])
                    Element("Parameter",[("Name","Rob_A");("Section","Input");("Type","Pointer")],"",[
                        Element("StringAttribute",[("Name","InterfaceFlags");("Informative","true")],"S7_Visible",[])
                    ])
                    Element("Parameter",[("Name","ST_ROB");("Section","InOut");("Type","ST_Rob")],"",[
                        Element("StringAttribute",[("Name","InterfaceFlags");("Informative","true")],"S7_Visible",[])
                    ])
                ])
            ])
        ])
        Element("Wires",[],"",[
            Element("Wire",[("UId","25")],"",[
                Element("Powerrail",[],"",[])
                Element("NameCon",[("UId","23");("Name","en")],"",[])
            ])
            Element("Wire",[("UId","26")],"",[
                Element("IdentCon",[("UId","21")],"",[])
                Element("NameCon",[("UId","23");("Name","Rob_a")],"",[])
            ])
            Element("Wire",[("UId","26")],"",[
                Element("IdentCon",[("UId","21")],"",[])
                Element("NameCon",[("UId","23");("Name","Rob_a")],"",[])
            ])
            Element("Wire",[("UId","27")],"",[
                Element("IdentCon",[("UId","22")],"",[])
                Element("NameCon",[("UId","23");("Name","ST_ROB")],"",[])
            ])
        ])
    ])


let blockCompileUnit (programmingLanguage:ProgrammingLanguage)=
    Element("SW.Blocks.CompileUnit",[("ID","3");("CompositionName","CompileUnits")],"",[
        Element("AttributeList",[],"",[
            Element("NetworkSource",[],"",[networkSource])
            Element("ProgrammingLanguage",[],programmingLanguage.GetValue,[])

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
            blockCompileUnit block.ProgrammingLanguage
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
        test "EmptyRobotFC" {
                let fcBlock  ={
                    Name = "ROBNAME"
                    Number  = 71
                    ProgrammingLanguage = LAD
                    Sections = sectionsRobo
                }
                let actual = createBlock ("EmptyRobotFC","V17",buildFcBlock fcBlock)
                let expected = Path.GetFullPath("templates/EmptyRobotFC.xml") |> File.ReadAllText
                Expect.equal actual expected "EmptyRobotFC file should match"
            }

    ]
let result = runTests defaultConfig (tests())
printfn "result %A" result