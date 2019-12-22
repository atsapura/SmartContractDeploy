module ContractDeploymentPipeline

open System.IO
open ContractDeploying
open ContractValidation
open FsHttp
open FsHttp.DslCE
open Utf8Json
open Utf8Json.Resolvers
open Utf8Json.FSharp

let sender = "CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54"

let basicUrl = "http://localhost:37223/api/smartcontractwallet"

let readContractSource filePath =
    try
        File.ReadAllText filePath |> Ok
    with
    | Failure f -> f |> FileError |> Error

let buildContractData contractCode =
    {
        SenderWalletName = "Hackathon_1"
        ContractCrsAmount = 123456m
        FeeAmount = 0.001m
        Password = "stratis"
        Parameters = [||]
        GasPrice = 100UL
        GasLimit = 100000UL
        ContractCode = contractCode
        SenderAddress = sender
    }

let parseContract filePath =
    result {
        let! sourceCode = readContractSource filePath
        let! compiledCode = validationPipeline sourceCode
        return buildContractData compiledCode.ByteCode |> createDeploymentRequest
    }

let createContractApiCall (request: ContractDeploymentReqiest) =
    let url = sprintf "%s/%s" basicUrl "create"
    let requestJson = request |> JsonSerializer.ToJsonString
    printfn "%s" requestJson
    http {
        POST url
        body
        json requestJson
    }

let parseAndDeploy filePath =
    result {
        let! deployRequest = parseContract filePath
        return createContractApiCall deployRequest
    }

