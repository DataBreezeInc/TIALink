module Docs.Pages.Block

open Feliz
open Elmish
open Docs.SharedView

let programmingLanguages =

    let code =
        """type ProgrammingLanguage =
        | LAD
        | FBD
        | DB
        | SCL
"""

    let title = Html.text "Currently following ProgrammingLanguages are supported."
    codedNoExampleView title code
let memoryLayout =

    let code =
        """type MemoryLayout =
        | Optimized
        | Standard
"""

    let title = Html.text "Currently following MemoryLayouts are supported."
    codedNoExampleView title code

let blockType =

    let code =
        """type BlockType =
    | GlobalDB of GlobalDB
    | OrganisationalBlock
    | FunctionalBlock of FCBlock
"""

    let title = Html.text "Currently the following block types are supported."
    codedNoExampleView title code
let functionalBlock =

    let code =
        """type FCBlock =
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
"""

    let title = Html.text "FunctionalBlock contains following elements."
    codedNoExampleView title code
let globalDb =

    let code =
        """type GlobalDB =
        { Name: string
          GlobalDBId: GlobalDBId
          ProgrammingLanguage: ProgrammingLanguage
          Sections: seq<Xml>
          MemoryLayout: MemoryLayout
          CreateTime: DateTime
          TiaVersion: TiaVersion
          Comment: string option
          Title: string option }
"""

    let title = Html.text "Global DataBlock contains following elements."
    codedNoExampleView title code


[<ReactComponent>]
let BlockView () =
    React.fragment [
        programmingLanguages
        memoryLayout
        blockType
        functionalBlock
        globalDb
    ]