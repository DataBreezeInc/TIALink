[<AutoOpen>]
module XmlHelper

open System.Xml
open System.IO
open System.Text
open System



type Xml =
    | Element of string * (string * string) seq * string * Xml seq
    member this.WriteContentTo(writer: XmlWriter) =
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
        using (new XmlTextWriter(new StringWriter(output), Formatting = Formatting.Indented)) this.WriteContentTo
        output.ToString()

type Language =
    | German
    | English
    | Spanish
    | French
    | Czech
    | Polnish
    | Chinese
    | Russian
    | Portuguese
    | Slovak
    | Dutch
    | Hungarian
    member this.Value =
        match this with
        | German -> "de-DE"
        | English -> "en-US"
        | Spanish -> "es-ES"
        | French -> "fr-FR"
        | Chinese -> "zh-CN"
        | Czech -> "cs-CZ"
        | Polnish -> "pl-PL"
        | Russian -> "ru-RU"
        | Portuguese -> "pt-BR"
        | Slovak -> "sk-SK"
        | Dutch -> "nl-BE"
        | Hungarian -> "hu-HU"

type UId =
    | UId of int
    member this.Value = (fun (UId id) -> id) this
    member this.ValueAsString = (fun (UId id) -> string id) this


type CompileUnitId =
    | CompileUnitId of string
    member this.Value = (fun (CompileUnitId id) -> id) this

type DataType =
    | Bool
    | Byte
    | Word
    | DWord
    | Int
    | DInt
    | UInt
    | UDInt
    | Real
    | S5Time
    | Time
    | Void
    | Struct
    | Custom of string
    member this.Value =
        match this with
        | Bool -> "Bool"
        | Byte -> "Byte"
        | Word -> "Word"
        | DWord -> "DWord"
        | Int -> "Int"
        | DInt -> "DInt"
        | UInt -> "UInt"
        | UDInt -> "UDInt"
        | Real -> "Real"
        | S5Time -> "S5Time"
        | Time -> "Time"
        | Void -> "Void"
        | Struct -> "Struct"
        | Custom str -> str

type TiaVersion =
    | V16
    | V17
    member this.Value =
        match this with
        | V16 -> "V16"
        | V17 -> "V17"




type Scope =
    | Global
    | Local
    member this.Value =
        match this with
        | Global -> "GlobalVariable"
        | Local -> "LocalVariable"


let creationTime (date: DateTime) = date.ToString("yyyy-MM-ddTHH:mm:ss")


[<RequireQualifiedAccess>]
module Product =

    let product productName version =
        Element(
            "Product",
            [],
            "",
            [ Element("DisplayName", [], productName, [])
              Element("DisplayVersion", [], version, []) ]
        )

    let optionPackage packageName version =
        Element(
            "OptionPackage",
            [],
            "",
            [ Element("DisplayName", [], packageName, [])
              Element("DisplayVersion", [], version, []) ]
        )

[<RequireQualifiedAccess>]
module Section =

    type Section =
        | Input
        | InOut
        | Output
        | Return
        | Temp
        | Constant
        | Static
        | NoSection
        member this.Value =
            match this with
            | Input -> "Input"
            | InOut -> "InOut"
            | Output -> "Output"
            | Return -> "Return"
            | Temp -> "Temp"
            | Constant -> "Constant"
            | Static -> "Static"
            | NoSection -> "None"

    type Accessibility =
        | Public
        | Private
        | LocalVariable
        | NoAccessibility
        member this.Value =
            match this with
            | Public -> "Public"
            | Private -> "Private"
            | LocalVariable -> "LocalVariable"
            | NoAccessibility -> ""

    type Remanence =
        | Retain
        | NoRemanence
        member this.Value =
            match this with
            | Retain -> "Retain"
            | NoRemanence -> ""

    let sections elements = Element("Sections", [], "", elements)

    let section (sectionName: Section) elements =
        Element("Section", [ ("Name", sectionName.Value) ], "", elements)

    let memberElement name (dataType: DataType) (remanence: Remanence) (accessibility: Accessibility) childElements =
        Element(
            "Member",
            [ match remanence with
              | NoRemanence ->
                  match accessibility with
                  | NoAccessibility ->
                      ("Name", name)
                      ("Datatype", dataType.Value)
                  | _ ->
                      ("Name", name)
                      ("Datatype", dataType.Value)
                      ("Accessibility", accessibility.Value)
              | _ ->
                  match accessibility with
                  | NoAccessibility ->
                      ("Name", name)
                      ("Remanence", remanence.Value)
                      ("Datatype", dataType.Value)
                  | _ ->
                      ("Name", name)
                      ("Datatype", dataType.Value)
                      ("Remanence", remanence.Value)
                      ("Accessibility", accessibility.Value) ],
            "",
            childElements
        )



[<RequireQualifiedAccess>]
module NetworkSource =

    type BlockType =
        | FB
        | FC
        member this.Value =
            match this with
            | FB -> "FB"
            | FC -> "FC"

    type AreaType =
        | Memory
        | Input
        | NoArea
        | DB
        member this.Value =
            match this with
            | Memory -> "Memory"
            | Input -> "Input"
            | NoArea -> "None"
            | DB -> "DB"

    type Access =
        { AreaType: AreaType
          DataType: DataType
          ComponentName: string
          UId: UId
          BitOffset: int option
          BlockNumber: int option
          Scope: Scope }
        member this.GetBitOffset =
            match this.BitOffset with
            | Some offset -> offset |> string
            | None -> ""

    type CallInfoName =
        | MUL
        | Custom of string
        member this.Value =
            match this with
            | MUL -> "MUL"
            | Custom str -> str

    type Call =
        { BlockName: string
          AreaType: AreaType
          CallInfoName: CallInfoName
          BlockType: BlockType
          UId: UId
          BitOffset: int
          BlockNumber: int
          InstanceBlockNumber: int
          CreateDate: DateTime
          Parameters: seq<Xml> }

    let parameter name (section: Section.Section) (dataType: DataType) =
        Element(
            "Parameter",
            [ ("Name", name)
              ("Section", section.Value)
              ("Type", dataType.Value) ],
            "",
            [ Element(
                  "StringAttribute",
                  [ ("Name", "InterfaceFlags")
                    ("Informative", "true") ],
                  "S7_Visible",
                  []
              ) ]
        )

    let accessElement (access: Access) =
        let blockContent =
            match access.BlockNumber with
            | Some bN ->
                [ ("Area", access.AreaType.Value)
                  ("Type", access.DataType.Value)
                  ("BlockNumber", bN |> string)
                  ("BitOffset", access.GetBitOffset)
                  ("Informative", "true") ]
            | None ->
                [ ("Area", access.AreaType.Value)
                  ("Type", access.DataType.Value)
                  ("BitOffset", access.GetBitOffset |> string)
                  ("Informative", "true") ]

        Element(
            "Access",
            [ ("Scope", access.Scope.Value)
              ("UId", access.UId.ValueAsString) ],
            "",
            [ Element(
                  "Symbol",
                  [],
                  "",
                  [ match access.Scope with
                    | Global ->
                        Element("Component", [ ("Name", access.ComponentName) ], "", [])
                        Element("Address", blockContent, "", [])
                    | Local -> Element("Component", [ ("Name", access.ComponentName) ], "", []) ]
              ) ]
        )

    let callElement (call: Call) =
        Element(
            "Call",
            [ ("UId", call.UId.ValueAsString) ],
            "",
            [ Element(
                  "CallInfo",
                  [ ("Name", call.CallInfoName.Value)
                    ("BlockType", call.BlockType.Value) ],
                  "",
                  [ match call.BlockType with
                    | FB ->
                        Element(
                            "IntegerAttribute",
                            [ ("Name", "BlockNumber")
                              ("Informative", "true") ],
                            call.BlockNumber |> string,
                            []
                        )

                        Element(
                            "DateAttribute",
                            [ ("Name", "ParameterModifiedTS")
                              ("Informative", "true") ],
                            call.CreateDate |> creationTime,
                            []
                        )

                        Element(
                            "Instance",
                            [ ("Scope", "GlobalVariable")
                              ("UId", (call.UId.Value + 1) |> string) ],
                            "",
                            [ Element(
                                  "Component",
                                  [ ("Name",
                                     call.BlockName
                                     + "#"
                                     + call.CallInfoName.Value
                                     + "_"
                                     + call.BlockType.Value) ],
                                  "",
                                  []
                              )
                              Element(
                                  "Address",
                                  [ ("Area", call.AreaType.Value)
                                    ("Type", call.CallInfoName.Value)
                                    ("BlockNumber", call.InstanceBlockNumber |> string)
                                    ("BitOffset", call.BitOffset |> string)
                                    ("Informative", "true") ],
                                  "",
                                  []
                              )

                              ]
                        )
                    | FC ->
                        Element(
                            "IntegerAttribute",
                            [ ("Name", "BlockNumber")
                              ("Informative", "true") ],
                            call.BlockNumber |> string,
                            []
                        )

                        Element(
                            "DateAttribute",
                            [ ("Name", "ParameterModifiedTS")
                              ("Informative", "true") ],
                            call.CreateDate |> creationTime,
                            []
                        )
                    for p in call.Parameters do
                        p ]
              ) ]
        )

    let templateValue =
        Element(
            "TemplateValue",
            [ ("Name", "Card")
              ("Type", "Cardinality") ],
            "2",
            []
        )

    let automaticTyped = Element("AutomaticTyped", [ ("Name", "SrcType") ], "", [])


    let networkSourceElement childElements =
        Element(
            "FlgNet",
            [ ("xmlns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v4") ],
            "",
            childElements
        )

    let parts childElements = Element("Parts", [], "", childElements)

    type Instruction =
        | Mul
        member this.Value =
            match this with
            | Mul -> "Mul"

    let part (instruction: Instruction) uid =
        Element(
            "Part",
            [ ("Name", instruction.Value)
              ("UId", uid) ],
            "",
            [ templateValue; automaticTyped ]
        )

[<RequireQualifiedAccess>]
module Wires =

    type WireType =
        | IdentCon of UId
        | PowerRail
        | OpenCon of UId

    type Wire =
        { UId: UId
          Name: string
          NameUId: UId
          WireType: WireType }

    let wireElement (wire: Wire) =
        Element(
            "Wire",
            [ ("UId", wire.UId.ValueAsString) ],
            "",
            [ match wire.WireType with
              | PowerRail -> Element("Powerrail", [], "", [])
              | IdentCon uId -> Element("IdentCon", [ ("UId", uId.ValueAsString) ], "", [])
              | OpenCon uId -> Element("OpenCon", [ ("UId", uId.ValueAsString) ], "", [])
              Element(
                  "NameCon",
                  [ ("UId", wire.NameUId.ValueAsString)
                    ("Name", wire.Name) ],
                  "",
                  []
              ) ]
        )

    let wires childElements = Element("Wires", [], "", childElements)

let interfaceElement sections =
    Element(
        "Interface",
        [],
        "",
        [ Element("Sections", [ ("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v5") ], "", sections) ]
    )

module Block =

    type FCBlockId =
        | FCBlockId of int
        member this.Value = (fun (FCBlockId id) -> string id) this

    type GlobalDBId =
        | GlobalDBId of int
        member this.Value = (fun (GlobalDBId id) -> string id) this

    type MemoryLayout =
        | Optimized
        | Standard
        member this.Value =
            match this with
            | Optimized -> "Optimized"
            | Standard -> "Standard"

    type ProgrammingLanguage =
        | LAD
        | FBD
        | DB
        | SCL
        member this.Value =
            match this with
            | LAD -> "LAD"
            | FBD -> "FBD"
            | DB -> "DB"
            | SCL -> "SCL"

    type FCBlock =
        { Name: string
          FCBlockId: FCBlockId
          CompileUnitId: CompileUnitId
          ProgrammingLanguage: ProgrammingLanguage
          Sections: seq<Xml>
          MemoryLayout: MemoryLayout
          NetworkSource: Xml option
          CreateTime: DateTime
          TiaVersion: TiaVersion
          Comment: string option
          Title: string option }

    type GlobalDB =
        { Name: string
          GlobalDBId: GlobalDBId
          ProgrammingLanguage: ProgrammingLanguage
          Sections: seq<Xml>
          MemoryLayout: MemoryLayout
          CreateTime: DateTime
          TiaVersion: TiaVersion
          Comment: string option
          Title: string option }

    type BlockType =
        | GlobalDB of GlobalDB
        | OrganisationalBlock
        | FunctionalBlock of FCBlock

    let attributeList text =
        Element(
            "AttributeList",
            [],
            "",
            [ Element("Culture", [], "en-US", [])
              Element("Text", [], text, []) ]
        )

    let multilingualTextItemElement (id: int) text =
        Element(
            "MultilingualTextItem",
            [ ("ID", id |> string)
              ("CompositionName", "Items") ],
            "",
            [ attributeList text ]
        )

    let multilingualTextElement (id: int) name text =
        Element(
            "MultilingualText",
            [ ("ID", id |> string)
              ("CompositionName", name) ],
            "",
            [ Element("ObjectList", [], "", [ multilingualTextItemElement (id + 1) text ]) ]
        )

    let commentElement (lang: Language) (comment: string) =
        Element("Comment", [], "", [ Element("MultiLanguageText", [ ("Lang", lang.Value) ], comment, []) ])

    let startValue (dataType: DataType) (value: obj) =
        let valueStr =
            match dataType with
            | Real -> sprintf "%f" (value :?> float)
            | Bool -> sprintf "%b" (value :?> bool)
            | Byte -> failwith "Not Implemented"
            | Word -> failwith "Not Implemented"
            | DWord -> failwith "Not Implemented"
            | Int -> sprintf "%i" (value :?> int)
            | DInt -> sprintf "%i" (value :?> int)
            | UInt -> sprintf "%i" (value :?> uint)
            | UDInt -> failwith "Not Implemented"
            | S5Time -> failwith "Not Implemented"
            | Time -> failwith "Not Implemented"
            | Void -> failwith "Not Implemented"
            | Struct -> failwith "Not Implemented"
            | Custom (_) -> failwith "Not Implemented"

        Element("StartValue", [], valueStr, [])


    let blockCompileUnit (fcBlock: FCBlock) =
        Element(
            "SW.Blocks.CompileUnit",
            [ ("ID", fcBlock.CompileUnitId.Value)
              ("CompositionName", "CompileUnits") ],
            "",
            [ Element(
                  "AttributeList",
                  [],
                  "",
                  [ match fcBlock.NetworkSource with
                    | Some networkSource -> Element("NetworkSource", [], "", [ networkSource ])
                    | None -> Element("NetworkSource", [], "", [])
                    Element("ProgrammingLanguage", [], fcBlock.ProgrammingLanguage.Value, [])

                    ]
              )
              Element(
                  "ObjectList",
                  [],
                  "",
                  [ multilingualTextElement 4 "Comment" (fcBlock.Comment |> Option.defaultValue "")
                    multilingualTextElement 6 "Title" (fcBlock.Title |> Option.defaultValue "") ]
              ) ]
        )



    let buildFcBlock (block: FCBlock) =
        Element(
            "SW.Blocks.FC",
            [ ("ID", block.FCBlockId.Value) ],
            "",
            [ Element(
                  "AttributeList",
                  [],
                  "",
                  [ Element("AutoNumber", [], "true", [])
                    Element("HeaderAuthor", [], "", [])
                    Element("HeaderFamily", [], "", [])
                    Element("HeaderName", [], "", [])
                    Element("HeaderVersion", [], "1.1", [])
                    interfaceElement block.Sections
                    Element("IsIECCheckEnabled", [], "false", [])
                    Element("MemoryLayout", [], block.MemoryLayout.Value, [])
                    Element("Name", [], block.Name, [])
                    Element("Number", [], block.FCBlockId.Value, [])
                    Element("ProgrammingLanguage", [], block.ProgrammingLanguage.Value, [])
                    Element("SetENOAutomatically", [], "false", [])
                    Element("StructureModified", [ ("ReadOnly", "true") ], block.CreateTime |> creationTime, [])
                    Element("UDABlockProperties", [], "", [])
                    Element("UDAEnableTagReadback", [], "false", [])

                    ]
              )
              Element(
                  "ObjectList",
                  [],
                  "",
                  [ multilingualTextElement 1 "Comment" (block.Comment |> Option.defaultValue "")
                    blockCompileUnit block
                    multilingualTextElement 8 "Title" (block.Title |> Option.defaultValue "") ]
              ) ]
        )

    let buildGlobalDB (globalDB: GlobalDB) =
        Element(
            "SW.Blocks.GlobalDB",
            [ ("ID", globalDB.GlobalDBId.Value) ],
            "",
            [ Element(
                  "AttributeList",
                  [],
                  "",
                  [ Element("AutoNumber", [], "true", [])
                    Element("DBAccessibleFromOPCUA", [], "true", [])
                    Element("HeaderAuthor", [], "", [])
                    Element("HeaderFamily", [], "", [])
                    Element("HeaderName", [], "", [])
                    Element("HeaderVersion", [], "1.1", [])
                    interfaceElement globalDB.Sections
                    Element("IsOnlyStoredInLoadMemory", [], "false", [])
                    Element("IsWriteProtectedInAS", [], "false", [])
                    Element("MemoryLayout", [], globalDB.MemoryLayout.Value, [])
                    Element("Name", [], globalDB.Name, [])
                    Element("Number", [], globalDB.GlobalDBId.Value |> string, [])
                    Element("ProgrammingLanguage", [], globalDB.ProgrammingLanguage.Value, [])

                    ]
              )
              Element(
                  "ObjectList",
                  [],
                  "",
                  [ multilingualTextElement 1 "Comment" (globalDB.Comment |> Option.defaultValue "")
                    multilingualTextElement 8 "Title" (globalDB.Title |> Option.defaultValue "") ]
              ) ]
        )

    let documentInfo (version: TiaVersion) block =
        Element(
            "Document",
            [],
            "",
            [ Element("Engineering", [ ("version", version.Value) ], "", [])
              Element(
                  "DocumentInfo",
                  [],
                  "",
                  [ Element("Created", [], "2021-09-08T12:46:16.1780561Z", [])
                    Element("ExportSetting", [], "WithDefaults", [])
                    Element(
                        "InstalledProducts",
                        [],
                        "",
                        [ Product.product "Totally Integrated Automation Portal" version.Value
                          Product.optionPackage "TIA Portal Openness" version.Value
                          Product.optionPackage "TIA Portal Version Control Interface" version.Value
                          Product.product "STEP 7 Professional" version.Value
                          Product.optionPackage "STEP 7 Safety" version.Value
                          Product.product "WinCC Advanced" version.Value ]
                    ) ]
              )
              match block with
              | FunctionalBlock fcBlock -> buildFcBlock fcBlock
              | GlobalDB globalDb -> buildGlobalDB globalDb
              | OrganisationalBlock -> failwith "Not Implemented" ]
        )

module PlcDataType =

    type DataTypeId =
        | DataTypeId of int
        member this.Value = (fun (DataTypeId id) -> string id) this

    type AttributeInfo =
        | ExternalAccessible
        | ExternalVisible
        | ExternalWritable
        | SetPoint
        member this.Value =
            match this with
            | ExternalAccessible -> "ExternalAccessible"
            | ExternalVisible -> "ExternalVisible"
            | ExternalWritable -> "ExternalWritable"
            | SetPoint -> "SetPoint"

        member this.Status =
            match this with
            | ExternalAccessible -> "true"
            | ExternalVisible -> "true"
            | ExternalWritable -> "true"
            | SetPoint -> "false"

    type PlcDataType =
        { Name: string
          Number: int
          DataTypeId: DataTypeId
          Sections: seq<Xml>
          CreationTime: DateTime }

    let attributeList =
        Element(
            "AttributeList",
            [],
            "",
            [ Element("Culture", [], "en-US", [])
              Element("Text", [], "", []) ]
        )

    let multilingualTextItemElement (id: int) =
        Element(
            "MultilingualTextItem",
            [ ("ID", id |> string)
              ("CompositionName", "Items") ],
            "",
            [ attributeList ]
        )

    let multilingualTextElement (id: int) name =
        Element(
            "MultilingualText",
            [ ("ID", id |> string)
              ("CompositionName", name) ],
            "",
            [ Element("ObjectList", [], "", [ multilingualTextItemElement (id + 1) ]) ]
        )



    let dataTypeAttribute (dataType: DataType) (attributeInfo: AttributeInfo) =
        match dataType with
        | Bool ->
            Element(
                "BooleanAttribute",
                [ ("Name", attributeInfo.Value)
                  ("SystemDefined", "true") ],
                attributeInfo.Status,
                []
            )
        | Byte -> failwith "Not Implemented"
        | Word -> failwith "Not Implemented"
        | DWord -> failwith "Not Implemented"
        | Int -> failwith "Not Implemented"
        | DInt -> failwith "Not Implemented"
        | UDInt -> failwith "Not Implemented"
        | UInt -> failwith "Not Implemented"
        | Real -> failwith "Not Implemented"
        | S5Time -> failwith "Not Implemented"
        | Time -> failwith "Not Implemented"
        | Void -> failwith "Not Implemented"
        | Struct -> failwith "Not Implemented"
        | Custom (_) -> failwith "Not Implemented"

    let attributeElement attributes =
        Element("AttributeList", [], "", attributes)

    let dataTypeContent (dataType: PlcDataType) =
        Element(
            "SW.Types.PlcStruct",
            [ ("ID", dataType.DataTypeId.Value) ],
            "",
            [ Element(
                  "AttributeList",
                  [],
                  "",
                  [ interfaceElement dataType.Sections
                    Element("IsFailsafeCompliant", [], "false", [])
                    Element("Name", [], dataType.Name, []) ]
              )
              Element(
                  "ObjectList",
                  [],
                  "",
                  [ multilingualTextElement 1 "Comment"
                    multilingualTextElement 3 "Title" ]
              ) ]
        )

    let documentInfo (version: TiaVersion) dataType =
        Element(
            "Document",
            [],
            "",
            [ Element("Engineering", [ ("version", version.Value) ], "", [])
              Element(
                  "DocumentInfo",
                  [],
                  "",
                  [ Element("Created", [], "2021-09-08T12:46:16.1780561Z", [])
                    Element("ExportSetting", [], "WithDefaults", [])
                    Element(
                        "InstalledProducts",
                        [],
                        "",
                        [ Product.product "Totally Integrated Automation Portal" version.Value
                          Product.optionPackage "TIA Portal Openness" version.Value
                          Product.optionPackage "TIA Portal Version Control Interface" version.Value
                          Product.product "STEP 7 Professional" version.Value
                          Product.optionPackage "STEP 7 Safety" version.Value
                          Product.product "WinCC Advanced" version.Value ]
                    ) ]
              )
              dataTypeContent dataType ]
        )