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

        let rec myFunc (n:Node) : unit -> MemberInfo list = 
            fun () ->
                n.Edges
                |> Seq.map( fun (edgeName, node) ->
                    let t' = ProvidedTypeDefinition(className=edgeName, baseType=None)
                    t'.AddMembersDelayed (myFunc node)
                    t' :> MemberInfo
                )
                |> Seq.toList
            

        let buildTypes (typeName : string) (myParam : string) = 
            let t = ProvidedTypeDefinition(className = typeName, baseType = None)
            provTy.AddMember t

            //t.AddMembersDelayed (myFunc startingNode)
            t.AddMembersDelayed ( fun _ ->
                let npql = new ProvidedTypeDefinition(className="NPQL", baseType=None)
                npql.AddMembersDelayed (fun _ ->
                    NPQL.entryPoints
                    |> Seq.toList
                )
                let domainType = new ProvidedTypeDefinition(className="DomainTypes", baseType=None)
                [ npql :> MemberInfo ; domainType :> MemberInfo]
            )

            t
        
        let parameters = [ ProvidedStaticParameter("myParameter", typeof<string>) ]
        do provTy.DefineStaticParameters(parameters, fun typeName args -> buildTypes typeName (args.[0] :?> string))
        do this.AddNamespace(ns, [ provTy ])
    end

[<TypeProviderAssembly>]
do ()
