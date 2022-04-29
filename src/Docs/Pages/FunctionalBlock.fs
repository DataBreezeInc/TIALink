module Docs.Pages.FunctionalBlock

open Feliz
open Elmish
open Docs.SharedView

let functionalBlock =
    let code =
        """let sectionsMultiply =
    [ Section.section
          Section.Input
          [ Section.memberElement "In1" Real Section.NoRemanence Section.Public []
            Section.memberElement "In2" Real Section.NoRemanence Section.Public [] ]
      Section.section Section.Output [ Section.memberElement "Out" Real Section.NoRemanence Section.Public [] ] ]

llet networkSourceMultiply =
    NetworkSource.networkSourceElement
        [ NetworkSource.parts [
            NetworkSource.accessElement
                                    { AreaType = NetworkSource.Memory
                                      ComponentName = "In1"
                                      UId = UId 21
                                      BitOffset = None
                                      BlockNumber = None
                                      DataType = Real
                                      Scope = Local }
            NetworkSource.accessElement
                                    { AreaType = NetworkSource.Memory
                                      ComponentName = "In2"
                                      UId = UId 22
                                      BitOffset = None
                                      BlockNumber = None
                                      DataType = Real
                                      Scope = Local }
            NetworkSource.accessElement
                                    { AreaType = NetworkSource.Memory
                                      ComponentName = "Out"
                                      UId = UId 23
                                      BitOffset = None
                                      BlockNumber = None
                                      DataType = Real
                                      Scope = Local }
            NetworkSource.part NetworkSource.Mul "24" ]
          Wires.wires [ Wires.wireElement
                            { UId = UId 25
                              Name = "en"
                              NameUId = UId 24
                              WireType = Wires.PowerRail }
                        Wires.wireElement
                            {   UId = UId 26
                                Name = "in1"
                                NameUId = UId 24
                                WireType = Wires.IdentCon(UId 21) }
                        Wires.wireElement
                            {   UId = UId 27
                                Name = "in2"
                                NameUId = UId 24
                                WireType = Wires.IdentCon(UId 22) }
                        Wires.wireElement
                              { UId = UId 28
                                Name = "out"
                                NameUId = UId 24
                                WireType = Wires.IdentCon(UId 23) }
                           ]
          ]
let fcBlockMultiply =
    { Name = "Multiply"
      FCBlockId = FCBlockId 0
      CompileUnitId = CompileUnitId "3"
      ProgrammingLanguage = LAD
      Sections = sectionsMultiply
      MemoryLayout = Optimized
      NetworkSource = Some(networkSourceMultiply)
      CreateTime = System.DateTime.Now
      TiaVersion = V17
      Title = Some "Multiply"
      Comment = Some "Some Comment" }

@"Your/Project/Path"
|> PlcProgram.projectPath
|> PlcProgram.selectProject "YourProjectName"
|> PlcProgram.getDevice ("6ES7 510-1DJ01-0AB0/V2.9","ET200SP")
|> PlcProgram.createFunctionalBlock fcBlockMultiply"""

    let title =
        Html.text
            "Creates a Mulitply Element with comment, titel and two inputs and one output."

    codedWithPictureView title code "./functionalblock_multiply.png"


[<ReactComponent>]
let FunctionalBlockView () =
    React.fragment [
        functionalBlock
    ]