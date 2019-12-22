// Learn more about F# at http://fsharp.org

open System
open FsHttp
open Utf8Json.Resolvers
open Utf8Json.FSharp

CompositeResolver.RegisterAndSetAsDefault(
  FSharpResolver.Instance,
  StandardResolver.CamelCase
)

[<EntryPoint>]
let main argv =
    let path = @"..\..\..\..\SmartContractsPlayground\KissContract.cs"
    printfn "Parsing contract [%s]" path
    match ContractDeploymentPipeline.parseAndDeploy path with
    | Ok response ->
        printfn "OOSPEKH!!!!!!"
        Console.WriteLine((response.statusCode, response.content.ReadAsStringAsync().Result))
    | Error e -> printfn "ERROR!\n%A" e
    printfn "Done"
    0 // return an integer exit code
