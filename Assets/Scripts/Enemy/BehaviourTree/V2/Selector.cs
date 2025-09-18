using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector() : base()
    {

    }
    public Selector(List<Node> children) : base(children)
    {

    }
    public override NodeState Evaluate()
    {
        foreach(var child in children)
        {
            switch(child.Evaluate())
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Running:
                    State = NodeState.Running;
                    return State;
                case NodeState.Failure:
                    continue;
            }
        }
        State = NodeState.Failure;
        return State;
    }
}
