module Docs.Pages.NetworkSource

open Feliz
open Elmish
open Docs.SharedView

let blockType =

    let code =
        """type BlockType =
            | FB
            | FC"""

    let title = Html.text "TIALink supports following Sections"
    codedNoExampleView title code

let areaType =

    let code =
        """type AreaType =
            | Memory
            | Input
            | NoArea
            | DB"""

    let title = Html.text "Following Accessibilities are supported"
    codedNoExampleView title code

let access =

    let code =
        """type Access =
            { AreaType: AreaType
            DataType: DataType
            ComponentName: string
            UId: UId
            BitOffset: int option
            BlockNumber: int option
            Scope: Scope }"""

    let title = Html.text "Following Remanences are supported"
    codedNoExampleView title code

let call =

    let code =
        """
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
         }"""

    let title = Html.text "Following Remanences are supported"
    codedNoExampleView title code

let sections =

    let code =
        """let sections =
    [ Section.section
          Section.Static
          [ Section.memberElement
                "Meter1"
                Real
                Section.Retain
                Section.Public
                [ startValue Real 100.
                  commentElement English "Inlet water consumption Meter1"
                   ]]]"""

    let title = Html.text "Creates a section XML Element"
    codedWithPictureView title code "./datablock.png"

[<ReactComponent>]
let NetworkSourceView () =
    React.fragment [ blockType
                     areaType
                     access
                     call
                     sections ]
