using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

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
                    break;
                case NodeState.Failure:
                    State = NodeState.Failure;
                    return State;
            }
        }
        State = anyChildRunning ? NodeState.Running : NodeState.Success;
        return State;
    }
}