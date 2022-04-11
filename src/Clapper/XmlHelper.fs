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

type UId =
    | UId of int
    member this.Value = (fun (UId id) -> id) this
    member this.ValueAsString = (fun (UId id) -> string id) this

type FCBlockId =
    | FCBlockId of int
    member this.Value = (fun (FCBlockId id) -> string id) this

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

type ProgrammingLanguage =
    | LAD
    | FBD
    member this.Value =
        match this with
        | LAD -> "LAD"
        | FBD -> "FBD"

type FCBlock =
    { Name: string
      Number: int
      FCBlockId: FCBlockId
      CompileUnitId: CompileUnitId
      ProgrammingLanguage: ProgrammingLanguage
      Sections: seq<Xml>
      NetworkSource: Xml option }


type Scope =
    | Global
    | Local
    member this.Value =
        match this with
        | Global -> "GlobalVariable"
        | Local -> "LocalVariable"





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
        member this.Value =
            match this with
            | Input -> "Input"
            | InOut -> "InOut"
            | Output -> "Output"
            | Return -> "Return"
            | Temp -> "Temp"
            | Constant -> "Constant"

    let section (sectionName: Section) elements =
        Element("Section", [ ("Name", sectionName.Value) ], "", elements)

    let memberElement name (dataType: DataType) accessibility childElements =
        Element(
            "Member",
            [ ("Name", name)
              ("Datatype", dataType.Value)
              ("Accessibility", accessibility) ],
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
            | None -> failwithf "bit offset not set"

    type Call =
        { BlockName: string
          AreaType: AreaType
          CallInfoName: string
          BlockType: BlockType
          UId: UId
          BitOffset: int
          BlockNumber: int
          InstanceBlockNumber: int
          CreateDate: DateTime
          Parameters: seq<Xml>

         }

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
                  [ ("Name", call.CallInfoName)
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
                            call.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss"),
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
                                     + call.CallInfoName
                                     + "_"
                                     + call.BlockType.Value) ],
                                  "",
                                  []
                              )
                              Element(
                                  "Address",
                                  [ ("Area", call.AreaType.Value)
                                    ("Type", call.CallInfoName)
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
                            call.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            []
                        )
                    for p in call.Parameters do
                        p ]
              ) ]
        )

    let parts childElements = Element("Parts", [], "", childElements)


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

[<RequireQualifiedAccess>]
module Block =

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
                  [ multilingualTextElement 4 "Comment"
                    multilingualTextElement 6 "Title" ]
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
                    Element(
                        "Interface",
                        [],
                        "",
                        [ Element(
                              "Sections",
                              [ ("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v5") ],
                              "",
                              block.Sections
                          ) ]
                    )
                    Element("IsIECCheckEnabled", [], "false", [])
                    Element("MemoryLayout", [], "Optimized", [])
                    Element("Name", [], block.Name, [])
                    Element("Number", [], block.Number |> string, [])
                    Element("ProgrammingLanguage", [], block.ProgrammingLanguage.Value, [])
                    Element("SetENOAutomatically", [], "false", [])
                    Element("StructureModified", [ ("ReadOnly", "true") ], "2017-06-20T14:56:00.5916118Z", [])
                    Element("UDABlockProperties", [], "", [])
                    Element("UDAEnableTagReadback", [], "false", [])

                    ]
              )
              Element(
                  "ObjectList",
                  [],
                  "",
                  [ multilingualTextElement 1 "Comment"
                    blockCompileUnit block
                    multilingualTextElement 8 "Title" ]
              ) ]
        )

    let documentInfo (version:TiaVersion) block =
        Element(
            "Document",
            [],
            "",
            [ Element("Engineering", [ ("version", version.Value) ], "", [])
              Element(
                  "DocumentInfo",
                  [],
                  "",
                  [ Element("Created", [], "2022-03-11T11:27:36.1676076Z", [])
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
              block ]
        )

