module Docs.View

open Feliz
open Router
open Elmish
open SharedView
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators

type Msg =
    | UrlChanged of Page
    | SetTheme of string

type State = { Page: Page; Theme: string }

let init () =
    let nextPage = Router.currentUrl () |> Page.parseFromUrlSegments
    { Page = nextPage; Theme = "dark" }, Cmd.navigatePage nextPage

let update (msg: Msg) (state: State) : State * Cmd<Msg> =
    match msg with
    | UrlChanged page -> { state with Page = page }, Cmd.none
    | SetTheme theme -> { state with Theme = theme }, Cmd.none

let private rightSide state dispatch (title: string) (docLink: string) elm =


    Daisy.drawerContent [ Daisy.navbar [ Daisy.navbarStart [ Html.divClassed
                                                                 "lg:hidden"
                                                                 [ Daisy.button.label [ button.square
                                                                                        button.ghost
                                                                                        prop.htmlFor "main-menu"
                                                                                        prop.children [ Svg.svg [ svg
                                                                                                                      .viewBox (
                                                                                                                          0,
                                                                                                                          0,
                                                                                                                          24,
                                                                                                                          24
                                                                                                                      )
                                                                                                                  svg.className
                                                                                                                      "inline-block w-6 h-6 stroke-current"
                                                                                                                  svg.children [ Svg.path [ svg.d
                                                                                                                                                "M4 6h16M4 12h16M4 18h16"
                                                                                                                                            svg.strokeWidth
                                                                                                                                                2 ] ] ] ] ] ] ] ]

                          Html.divClassed
                              "px-5 py-5"
                              [ Html.h2 [ color.textPrimary
                                          ++ prop.className "my-6 text-5xl font-bold"
                                          prop.text title ]

                                elm ] ]

let private leftSide (p: Page) =
    let miBadge (b: string) (t: string) (mp: Page) =
        Html.li [ Html.a [ prop.href mp
                           prop.onClick Router.goToUrl
                           if p = mp then
                               (menuItem.active
                                ++ prop.className "justify-between")
                           else
                               prop.className "justify-between"
                           prop.children [ Html.span t
                                           Html.span [ prop.className "badge"
                                                       prop.text b ] ] ] ]

    let mi (t: string) (mp: Page) =
        Html.li [ Html.a [ if p = mp then menuItem.active
                           prop.text t
                           prop.href mp
                           prop.onClick Router.goToUrl ] ]

    Daisy.drawerSide [ Daisy.drawerOverlay [ prop.htmlFor "main-menu" ]
                       Html.aside [ prop.className "flex flex-col border-r w-80 bg-base-100 text-base-content"
                                    prop.children [ Html.divClassed
                                                        "inline-block text-3xl font-title px-5 py-5 font-bold"
                                                        [ Html.span [ color.textPrimary
                                                                      prop.text "TIALink" ]
                                                          Html.a [ prop.href "https://www.nuget.org/packages/TIALink"
                                                                   prop.children [ Html.img [ prop.src
                                                                                                  "https://img.shields.io/nuget/v/TIALink.svg?style=flat-square" ] ] ] ]
                                                    Daisy.menu [ menu.compact
                                                                 prop.className "flex flex-col p-4 pt-0"
                                                                 prop.children [ Daisy.menuTitle [ Html.span "Docs" ]
                                                                                 mi "Install" Install
                                                                                 mi "Use" Use ] ]
                                                    Daisy.menu [ menu.compact
                                                                 prop.className "flex flex-col p-4 pt-0"
                                                                 prop.children [ Daisy.menuTitle [ Html.span
                                                                                                       "Configration" ]
                                                                                 miBadge
                                                                                     "New"
                                                                                     "Languages"
                                                                                     Languages
                                                                                  ] ]
                                                    Daisy.menu [ menu.compact
                                                                 prop.className "flex flex-col p-4 pt-0"
                                                                 prop.children [ Daisy.menuTitle [ Html.span
                                                                                                       "Operations" ]
                                                                                 miBadge "New" "PlcProps" PlcProps
                                                                                 miBadge "New" "Project" Project
                                                                                 miBadge "New" "Device" Device
                                                                                 miBadge
                                                                                     "New"
                                                                                     "HardwareObjects"
                                                                                     HardwareObjects
                                                                                 miBadge "New" "TagTable" TagTable
                                                                                 miBadge
                                                                                     "New"
                                                                                     "PlcDataTypeOp"
                                                                                     PlcDataTypeOp
                                                                                 miBadge
                                                                                     "New"
                                                                                     "DataBlock"
                                                                                     DataBlock

                                                                                 miBadge
                                                                                     "New"
                                                                                     "FunctionalBlock"
                                                                                     FunctionalBlock
                                                                                 miBadge "New" "Compile" Compile
                                                                                 miBadge "New" "Export" Export
                                                                                 miBadge "New" "Save" Save     ] ]
                                                    Daisy.menu [ menu.compact
                                                                 prop.className "flex flex-col p-4 pt-0"
                                                                 prop.children [ Daisy.menuTitle [ Html.span "XmlHelper" ]
                                                                                 miBadge
                                                                                     "New"
                                                                                     "DataTypes"
                                                                                     DataTypes
                                                                                 miBadge "New" "Section" Section
                                                                                 miBadge
                                                                                     "New"
                                                                                     "NetworkSource"
                                                                                     NetworkSource
                                                                                 miBadge "New" "Wires" Wires
                                                                                 miBadge "New" "Block" Block
                                                                                 miBadge
                                                                                     "New"
                                                                                     "PlcDataType"
                                                                                     PlcDataType ] ] ] ] ]

let private inLayout state dispatch (title: string) (docLink: string) (p: Page) (elm: ReactElement) =
    Html.div [ prop.className "bg-base-100 text-base-content h-screen"
               theme.custom state.Theme
               prop.children [ Daisy.drawer [ drawer.mobile
                                              prop.children [ Daisy.drawerToggle [ prop.id "main-menu" ]
                                                              rightSide state dispatch title docLink elm
                                                              leftSide p ] ] ] ]



[<ReactComponent>]
let AppView (state: State) (dispatch: Msg -> unit) =

    let title, docLink, content =
        match state.Page with
        | Install -> "Installation", "/docs/install", Pages.Install.InstallView()
        | Use -> "How to use", "/docs/use", Pages.Use.UseView()
        | Languages -> "Languages", "/configuration/languages", Pages.Languages.LanguagesView()
        | DataTypes -> "DataTypes", "/xmlHelper/DataTypes", Pages.DataTypes.DataTypesView()
        | Section -> "Section", "/xmlHelper/Section", Pages.Section.SectionView()
        | NetworkSource -> "NetworkSource", "/xmlHelper/networksource", Pages.NetworkSource.NetworkSourceView()
        | Wires -> "Wires", "/xmlHelper/wires", Pages.Badge.WiresView()
        | Block -> "Block", "/xmlHelper/block", Pages.Block.BlockView()
        | PlcDataType -> "PlcDataType", "/xmlHelper/plcdatatype", Pages.PlcDataType.PlcDataTypeView()
        | Compile -> "Compile", "/operations/compile", Pages.Compile.CompileView()
        | Export -> "Export", "/operations/export", Pages.Export.ExportView()
        | Save -> "Save", "/operations/Save", Pages.Save.SaveView()
        | PlcProps -> "PlcProps", "/operations/PlcProps", Pages.PlcProps.PlcPropsView()
        | Project -> "Project", "/operations/project", Pages.Project.ProjectView()
        | Device -> "Device", "/operations/device", Pages.Device.DeviceView()
        | HardwareObjects ->
            "HardwareObjects", "/operations/hardwareobjects", Pages.HardwareObjects.HardwareObjectsView()
        | TagTable -> "TagTable", "/operations/tagtable", Pages.TagTable.TagTableView()
        | PlcDataTypeOp -> "PlcDataTypeOp", "/operations/plcdatatypeop", Pages.PlcDataTypeOp.PlcDataTypeOpView()
        | DataBlock -> "DataBlock", "/operations/datablock", Pages.DataBlock.DataBlockView()
        | FunctionalBlock ->
            "FunctionalBlock", "/operations/functionalblock", Pages.FunctionalBlock.FunctionalBlockView()

    React.router [ router.hashMode
                   router.onUrlChanged (
                       Page.parseFromUrlSegments
                       >> UrlChanged
                       >> dispatch
                   )
                   router.children [ content
                                     |> inLayout state dispatch title docLink state.Page ] ]
