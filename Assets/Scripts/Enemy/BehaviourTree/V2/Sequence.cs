using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence() : base() { }

    public Sequence(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        var anyChildRunning = false;
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildRunning = true;
                    continue; // Changed from break to continue to check remaining children
                case NodeState.Failure:
                    State = NodeState.Failure;
                    return State;
            }
        }
        State = anyChildRunning ? NodeState.Running : NodeState.Success;
        return State;
    }
}