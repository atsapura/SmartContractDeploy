[<AutoOpen>]
module Common

open Microsoft.CodeAnalysis

type ResultBuilder() =
    member this.Bind(x,f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
    member this.Return x = Ok x
    member this.ReturnFrom x = x
    member thix.Zero() = Ok()

let result = ResultBuilder()

type ContractDeploymentError =
    | FileError of string
    | CompilationError of Diagnostic seq
    | FormatInvalid of string seq
    | NonDeterministic of string seq
    | WarningsDetected of string seq

type CompiledContract =
    {
        //Hash: int64
        ByteCode: byte[]
    }
