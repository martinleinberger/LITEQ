namespace LITEQ

open NPQL
open TypeMapper
open SchemaInformation
open SPARQLHttpProtocol

open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Collections
open ProviderImplementation.ProvidedTypes
open System.Reflection

[<TypeProvider>]
type RDFTypeProvider(config : TypeProviderConfig) as this = 
    class
        inherit TypeProviderForNamespaces()
        let ns = "Uniko.West"
        let asm = Assembly.GetExecutingAssembly()
        let provTy = ProvidedTypeDefinition(asm, ns, "RDFStore", Some typeof<obj>)
        
        let buildTypes (typeName : string) (myParam : string) = 
            let t = ProvidedTypeDefinition(className = typeName, baseType = None)
            provTy.AddMember t
            t
        
        let parameters = [ ProvidedStaticParameter("myParameter", typeof<string>) ]
        do provTy.DefineStaticParameters(parameters, fun typeName args -> buildTypes typeName (args.[0] :?> string))
        do this.AddNamespace(ns, [ provTy ])
    end

[<TypeProviderAssembly>]
do ()
