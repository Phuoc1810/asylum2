using System.Collections.Generic;
using UnityEngine;

public class Sequence: Node
{
    public Sequence() : base()
    {
    }
    public Sequence(List<Node> children) : base(children)
    {
    }
    public override NodeState Evaluate()
    {
        var aniChildren = false;
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    aniChildren = true;
                    break;
                case NodeState.Failure:
                    return NodeState.Failure;
            }
        }
        State = aniChildren ? NodeState.Running : NodeState.Success;
        return State;
    }
}
