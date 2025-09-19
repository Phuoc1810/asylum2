using System.Collections.Generic;
using UnityEngine;

public enum NodeState { Running, Success, Failure }

public abstract class Node
{
    public NodeState State { get; set; }
    public Node parent { get; set; }
    protected List<Node> children = new List<Node>();

    public Node()
    {
        State = NodeState.Failure; // Default state
    }

    public Node(List<Node> children)
    {
        if (children == null)
        {
            this.children = new List<Node>();
        }
        else
        {
            foreach (Node node in children)
            {
                if (node != null)
                    Attach(node);
            }
        }
    }

    public void Attach(Node child)
    {
        if (child != null)
        {
            child.parent = this;
            children.Add(child);
        }
    }

    public abstract NodeState Evaluate();
}