module ContractDeploying

open System
open NBitcoin

type ContractAddress = Stratis.SmartContracts.Address

type ContractParameter =
    | Boolean of bool
    | Byte of byte
    | Char of char
    | String of string
    | UInt32 of uint32
    | Int32 of int32
    | UInt64 of uint64
    | Int64 of int64
    | SmartContractAddress of string
    | ByteArray of byte[]

let private format typeNumber payload = payload.ToString() |> sprintf "%i#%O" typeNumber

type ContractDeploymentData =
    {
        SenderWalletName: string
        ContractCrsAmount: decimal
        FeeAmount: decimal
        Password: string
        GasPrice: uint64
        GasLimit: uint64
        ContractCode: byte[]
        SenderAddress: string//ContractAddress
        Parameters: ContractParameter []
    }

[<CLIMutable>]
type ContractDeploymentReqiest =
    {
        WalletName: string
        Amount: decimal
        FeeAmount: decimal
        Password: string
        GasPrice: uint64
        GasLimit: uint64
        Sender: string
        Parameters: string []
        ContractCode: string
    }

let private toBase58address network (address: ContractAddress) =
    let key = address.ToBytes() |> uint160 |> KeyId
    BitcoinPubKeyAddress(key, network).ToString()

let byteArrayToString bytes = BitConverter.ToString(bytes).Replace("-", "")

let private serialize = function
    | Boolean b -> format 1 b
    | Byte b -> format 2 b
    | Char c -> format 3 c
    | String s -> format 4 s
    | UInt32 u -> format 5 u
    | Int32 i -> format 6 i
    | UInt64 u -> format 7 u
    | Int64 i -> format 8 i
    | SmartContractAddress a ->
        //let publicKey = toBase58address network a
        format 9 a//publicKey
    | ByteArray b -> byteArrayToString b |> format 10

let createDeploymentRequest deploymentData =
    {
        WalletName = deploymentData.SenderWalletName
        Amount = deploymentData.ContractCrsAmount// |> string
        FeeAmount = deploymentData.FeeAmount// |> string
        GasLimit = deploymentData.GasLimit
        GasPrice = deploymentData.GasPrice
        Password = deploymentData.Password
        ContractCode = deploymentData.ContractCode |> byteArrayToString
        Sender = deploymentData.SenderAddress// |> toBase58address network
        Parameters = deploymentData.Parameters |> Array.map (serialize )
    }
