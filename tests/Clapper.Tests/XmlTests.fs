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

let product productName =
    Element("Product",[],"",[
        Element("DisplayName",[],productName,[])
        Element("DisplayVersion",[],"V17",[])
    ])
let optionPackage packageName =
    Element("OptionPackage",[],"",[
        Element("DisplayName",[],packageName,[])
        Element("DisplayVersion",[],"V17",[])
    ])

let section name elements =
    Element("Section",[("Name",name)],"",elements)

let sectionElement name dataType accessibility =
    Element("Member",[("Name",name);("Datatype",dataType);("Accessibility",accessibility)],"",[])
// let objectList (id:int) attributes =
//     Element("ObjectList",[],"",[
//             Element("MultilingualText",[("ID",id |> string);("CompositionName","Comment")],"",attributes)
//     ])

let attributeList =
    Element("AttributeList",[],"",[
        Element("Culture",[],"en-US",[])
        Element("Test",[],"",[])
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

let blockCompileUnit =
    Element("SW.Blocks.CompileUnit",[("ID","3");("CompositionName","CompileUnit")],"",[
        Element("AttributeList",[],"",[
            Element("NetworkSource",[],"",[])
            Element("ProgrammingLanguage",[],"FBD",[])

        ])
        Element("ObjectList",[],"",[
            multilingualTextElement 4 "Comment"
            multilingualTextElement 6 "Title"
        ])
    ])
let fcBlock =
    Element("SW.Blocks.FC",[("ID","0")],"",[
        Element("AttributeList",[],"",[
            Element("AutoNumber",[],"true",[])
            Element("HeaderAuthor",[],"",[])
            Element("HeaderFamily",[],"",[])
            Element("HeaderName",[],"",[])
            Element("HeaderVersion",[],"0.1",[])
            Element("Interface",[],"",[
                Element("Sections",[("xmlns","http://www.siemens.com/automation/Openness/SW/Interface/v5")],"",[
                    section "Input" [
                        sectionElement "input0" "Bool" "Public"
                        sectionElement "input1" "Bool" "Public"
                        sectionElement "input2" "Bool" "Public"
                        sectionElement "input3" "Bool" "Public"
                    ]
                    section "Output" [
                        sectionElement "output0" "Bool" "Public"
                        sectionElement "output1" "Bool" "Public"
                        sectionElement "output2" "Bool" "Public"
                        sectionElement "output3" "Bool" "Public"
                    ]
                    section "InOut" []
                    section "Temp" []
                    section "Constant" []
                    section "Return" [
                        sectionElement "Rel_Val" "Void" "Public"
                    ]
                ])
            ])
            Element("IsIECCheckEnabled",[],"false",[])
            Element("MemoryLayout",[],"Optimizes",[])
            Element("Name",[],"Inputs_1",[])
            Element("Number",[],"2",[])
            Element("ProgrammingLanguage",[],"FBD",[])
            Element("SetENOAutomatically",[],"false",[])
            Element("UDABlockProperties",[],"",[])
            Element("UDAEnableTagReadback",[],"false",[])

        ])
        Element("ObjectList",[],"",[
            multilingualTextElement 1 "Comment"
            blockCompileUnit
            multilingualTextElement 8 "Title"
        ])
    ])


let documentInfo =
    Element("Document",[],"",[
            Element("Engineering",[("version","V17")],"",[])
            Element("DocumentInfo",[],"",[
                Element("Created",[],"2022-03-11T11:27:36.1676076Z",[])
                Element("ExportSetting",[],"WithDefaults",[])
                Element("InstalledProducts",[],"",[
                    product "Totally Integrated Automation Portal"
                    optionPackage "TIA Portal Openness"
                    optionPackage "TIA Portal Version Control Interface"
                    product "STEP 7 Professional"
                    optionPackage "STEP 7 Safety"
                    product "WinCC Advanced"
                ])
            ])
            fcBlock
        ])
let doc =  XDocument(XElement.Parse(documentInfo.ToString()))
doc.Save(Path.GetFullPath("./Test.xml"))


let tests () =
    test "Input_1" {
            let actual = doc.ToString()
            let expected = Path.GetFullPath("Inputs_1.xml") |> File.ReadAllText
            Expect.equal actual expected "Process Excel file to produce expected MSCONS"
        }
let result = runTests defaultConfig (tests())
printfn "result %A" result