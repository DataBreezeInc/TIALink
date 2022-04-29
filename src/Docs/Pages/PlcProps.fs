module Docs.Pages.PlcProps

open Feliz
open Elmish
open Feliz.UseElmish
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Docs.SharedView

let ex1 =
    let exampleCode =
        """@"C:\Users\TimForkmann\Documents\Automatisierung\"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "ESA Kuwait"""

    let code =
        """type PlcProps =
    private
      { ExistingTiaPortalConnection: TiaPortal option
        Project: Project option
        Device: Device option
        PlcSoftware: PlcSoftware option
        PlcBlock: PlcBlock option
        ProjectName: string
        UserInterface: bool
        ProjectPath: string
        DeviceItems: DeviceItem []
        TagTableList: PlcTagTable []
        PlcTypeList: PlcType [] }
]"""

    let title = Html.text "Contains all the properties of the PLC."
    codedWithTextExampleView title code exampleCode


[<ReactComponent>]
let PlcPropsView () = React.fragment [ ex1 ]