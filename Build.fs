open Fake
open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.Core.TargetOperators
open System
open System.IO

open BuildHelpers
open BuildTools

initializeContext ()

let publishPath = Path.getFullName "publish"
let srcPath = Path.getFullName "src"
let clientSrcPath = srcPath </> "Docs"
let librarySrcPath = srcPath </> "TIALink"
let appPublishPath = publishPath </> "app"

// Targets
let clean proj =
    [ proj </> "bin"; proj </> "obj" ]
    |> Shell.cleanDirs

let createNuget proj =
    clean proj
    Tools.dotnet "restore --no-cache" proj
    Tools.dotnet "pack -c Release" proj

let getBuildParam = Environment.environVar
let isNullOrWhiteSpace = String.IsNullOrWhiteSpace

Target.create "InstallClient" (fun _ ->
    printfn "Node version:"
    Tools.node "--version" clientSrcPath
    printfn "Yarn version:"
    Tools.yarn "--version" clientSrcPath
    Tools.yarn "install --frozen-lockfile" clientSrcPath
)

let publishNuget proj =
    createNuget proj

    let nugetKey =
        match getBuildParam "nugetkey" with
        | s when not (isNullOrWhiteSpace s) -> s
        | _ -> UserInput.getUserPassword "NuGet Key: "

    let nupkg =
        Directory.GetFiles(proj </> "bin" </> "Release")
        |> Seq.head
        |> Path.GetFullPath

    Tools.dotnet (sprintf "nuget push %s -s nuget.org -k %s" nupkg nugetKey) proj

Target.create "Pack" (fun _ -> createNuget librarySrcPath)

Target.create "Publish" (fun _ -> publishNuget librarySrcPath)

Target.create "PublishDocs" (fun _ -> Tools.yarn "build" "")

Target.create "Run" (fun _ -> Tools.yarn "start" "")

let dependencies =
    [ "InstallClient" ==> "PublishDocs"
      "InstallClient" ==> "Publish"
      "InstallClient" ==> "Pack"
      "InstallClient" ==> "Run" ]

[<EntryPoint>]
let main args = runOrDefault "Run" args
