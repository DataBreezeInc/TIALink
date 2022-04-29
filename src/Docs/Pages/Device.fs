module Docs.Pages.Device

open Feliz
open Elmish
open Docs.SharedView

let device =
    let code =
        """@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.addAllLanguages
|> PlcProgram.getDevice ("6ES7 510-1DJ01-0AB0/V2.9","ET200SP")"""

    let title =
        Html.text
            "Select a device (Plc) by using the Device Id and the Device Name. Creates a new Devuce if not existing"

    codedWithPictureView title code "./device.png"

[<ReactComponent>]
let DeviceView () = React.fragment [ device ]