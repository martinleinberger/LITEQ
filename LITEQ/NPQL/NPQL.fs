module NPQL

open SchemaInformation
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Collections
open ProviderImplementation.ProvidedTypes
open System.Reflection
open System.Collections.Generic

let store = DummyImpl() :> ISchema
let typeCache = new Dictionary<string, ProvidedTypeDefinition>()

let rec internal makeMethods (edges : Edge seq) = 
    edges
    |> Seq.map (fun (edgeName, targetNode) -> 
           let t = makeClass targetNode
           ProvidedProperty(edgeName, t, GetterCode = fun _ -> <@@ new obj() @@>))
    |> Seq.toList

and internal makeClass (node : Node) = 
    if typeCache.ContainsKey node.Uri
        then typeCache.[node.Uri]
        else 
            let t = ProvidedTypeDefinition(className = node.Name, baseType = None)
            t.AddXmlDoc node.Comment
            //            t.AddMembersDelayed( fun _ ->
            //                makeMethods node.Edges
            //            )
            typeCache.[node.Uri] <- t
            t

let entryPoints = store.StartingNodes |> Seq.map makeClass
