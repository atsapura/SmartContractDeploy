module ContractValidation

open Microsoft.CodeAnalysis
open Stratis.SmartContracts.CLR.Compilation
open Stratis.SmartContracts.CLR.Validation

let private determinismValidator = SmartContractDeterminismValidator()
let private formatValidator = SmartContractFormatValidator()
let private warningValidator = SmartContractWarningValidator()

let private wrapCompilationResult (compilationResult: ContractCompilationResult) =
    if compilationResult.Success then
        { ByteCode = compilationResult.Compilation } |> Ok
    else
        compilationResult.Diagnostics |> Error

let private validateCsharpContractCode (code: string) =
    ContractCompiler.Compile(code, OptimizationLevel.Release) |> wrapCompilationResult

let private moduleDefinition compilation =
    use assemblyResolver = new DotNetCoreAssemblyResolver()
    let moduleDefinition = ContractDecompiler.GetModuleDefinition(compilation, assemblyResolver).Value
    moduleDefinition.ModuleDefinition

let private validate (validator: ISmartContractValidator, errorCtor) moduleDefinition =
    let result = validator.Validate(moduleDefinition)
    if result.IsValid then Ok ()
    else result.Errors |> Seq.map (fun v -> v.Message) |> errorCtor |> Error

let private validateFormat = validate (formatValidator, FormatInvalid)

let private validateDeterminism = validate (determinismValidator, NonDeterministic)

let private validateWarnings = validate (warningValidator, WarningsDetected)

let validationPipeline sourceCode =
    result {
        let! compilation = validateCsharpContractCode sourceCode |> Result.mapError CompilationError
        let moduleDefinition = moduleDefinition compilation.ByteCode
        do! validateFormat moduleDefinition
        do! validateDeterminism moduleDefinition
        do! validateWarnings moduleDefinition
        return compilation
    }
