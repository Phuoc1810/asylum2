using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState { Running, Success, Failure }
public abstract class Node
{
    public NodeState State;
    public Node parent;
    protected List<Node> children = new List<Node>();
    public Node()
    {

    }
    public Node(List<Node> children)
    {
        foreach (Node node in children)
        {
            Attach(node);
        }
    }
    public void Attach(Node child)
    {
        child.parent = this;
        children.Add(child);
    }
    public abstract NodeState Evaluate();
}
