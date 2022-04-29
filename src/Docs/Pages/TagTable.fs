module Docs.Pages.TagTable

open Feliz
open Elmish
open Docs.SharedView

let addTagTable =

    let code = """

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.addTagTable "Tag List Name"
)"""
    let title = Html.text "Add a TagList to your TIA project."
    codedWithPictureView title code "./taglist.png"
let addTag =

    let code = """

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.addTag (
    { Name = "E0.0"
      DataType = Bool
      Comment = "air blower 1"
      Address = "%I10.1" },
    "Tag List Name"
)"""
    let title = Html.text "Selects a TagList and adds a list of tags to your TIA project."
    codedWithPictureView title code "./addtag.png"
let addTags =

    let code = """

let tags =
    [ "E0.0", Bool, "air blower 1","%I0.0"
      "E0.1", Bool, "air blower 2","%I0.1""
      "E0.2", Bool, "air blower 3","%I0.2"
      ]
    |> List.mapi (fun i (x,y,z,a) ->    {
                                        Name = x
                                        DataType = y
                                        Comment =z
                                        Address = a
                                })

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.addTags (tags,"Tag List Name")"""
    let title = Html.text "Create a new TagList with a list of tags to your TIA project."
    codedWithPictureView title code "./addtags.png"

[<ReactComponent>]
let TagTableView () =

    React.fragment [
        addTagTable
        addTag
        addTags]