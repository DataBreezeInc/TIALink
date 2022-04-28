module Docs.Router

open Browser.Types
open Feliz.Router
open Fable.Core.JsInterop

type Page =
    | Install
    | Use
    | Languages
    | DataTypes
    | Section
    | NetworkSource
    | Wires
    | Block
    | PlcDataType
    | PlcProps
    | Project
    | Device
    | HardwareObjects
    | TagTable
    | PlcDataTypeOp
    | PlcBlock
    | DataBlock

[<RequireQualifiedAccess>]
module Page =
    let defaultPage = Page.Install

    let parseFromUrlSegments = function
        | [ "use" ] -> Page.Use
        | [ "languages" ] -> Page.Languages
        | [ "datatypes" ] -> Page.DataTypes
        | [ "section" ] -> Page.Section
        | [ "networksource" ] -> Page.NetworkSource
        | [ "wires" ] -> Page.Wires
        | [ "block" ] -> Page.Block
        | [ "plcdatatype" ] -> Page.PlcDataType
        | [ "plcprops" ] -> Page.PlcProps
        | [ "project" ] -> Page.Project
        | [ "device" ] -> Page.Device
        | [ "hardwareobjects" ] -> Page.HardwareObjects
        | [ "tagtable" ] -> Page.TagTable
        | [ "plcdatatypeop" ] -> Page.PlcDataTypeOp
        | [ "plcblock" ] -> Page.PlcBlock
        | [ "datablock" ] -> Page.DataBlock
        | [ ] -> Page.Install
        | _ -> defaultPage

    let noQueryString segments : string list * (string * string) list = segments, []

    let toUrlSegments = function
        | Page.Install -> [ ] |> noQueryString
        | Page.Use -> [ "use" ] |> noQueryString
        | Page.Languages -> [ "languages" ] |> noQueryString
        | Page.DataTypes -> [ "datatypes" ] |> noQueryString
        | Page.Section -> [ "section" ] |> noQueryString
        | Page.NetworkSource -> [ "networksource" ] |> noQueryString
        | Page.Wires -> [ "wires" ] |> noQueryString
        | Page.Block -> [ "block" ] |> noQueryString
        | Page.PlcDataType -> [ "plcdatatype" ] |> noQueryString
        | Page.PlcProps -> [ "plcprops" ] |> noQueryString
        | Page.Project -> [ "project" ] |> noQueryString
        | Page.Device -> [ "device" ] |> noQueryString
        | Page.HardwareObjects -> [ "hardwareobjects" ] |> noQueryString
        | Page.TagTable -> [ "tagtable" ] |> noQueryString
        | Page.PlcDataTypeOp -> [ "plcdatatypeop" ] |> noQueryString
        | Page.PlcBlock -> [ "plcblock" ] |> noQueryString
        | Page.DataBlock -> [ "datablock" ] |> noQueryString

[<RequireQualifiedAccess>]
module Router =
    let goToUrl (e:MouseEvent) =
        e.preventDefault()
        let href : string = !!e.currentTarget?attributes?href?value
        Router.navigate href

    let navigatePage (p:Page) = p |> Page.toUrlSegments |> Router.navigate

[<RequireQualifiedAccess>]
module Cmd =
    let navigatePage (p:Page) = p |> Page.toUrlSegments |> Cmd.navigate