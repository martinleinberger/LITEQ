module SchemaInformation

open System

type Uri = string
type Name = string
type Comment = string



type Node(name, ?comment) =
    let comment' = defaultArg comment "<summary>No further documentation available</summary>" 
    abstract member Uri : Uri
    abstract member Name : string
    abstract member Comment : Comment
    abstract member Edges : Edge seq

    default this.Uri = this.Name
    default this.Comment = comment'
    default this.Name = name
    default this.Edges = Seq.empty<Edge>
and Edge = string*Node

type ISchema = 
    interface
        abstract StartingNodes : Node seq
        abstract Edges : Node -> Edge seq
    end

type DummyImpl() = class
    interface ISchema with
        member this.StartingNodes =
            [for x in [1..5] do
                yield StartingNode(string x) :> Node]
            |> Seq.ofList
        member this.Edges n = n.Edges
end
and internal StartingNode(name) = class
    inherit Node(name) 
    
    override this.Edges = 
        [for x in [1..10] do
            yield "edge"+string x, ChildNode(string x) :> Node]
        |> Seq.ofList
end
and internal ChildNode(name) = class
    inherit Node(name)
end






























