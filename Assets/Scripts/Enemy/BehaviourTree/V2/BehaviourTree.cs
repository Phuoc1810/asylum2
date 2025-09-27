using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree(string name = "BehaviorTree") : base(name)
    {
    }

    public override NodeStatus Process()
    {
        // Trong một BehaviorTree đơn giản, nó có thể chỉ xử lý nút con đầu tiên
        // hoặc xử lý tất cả các nút con theo một logic nhất định.
        // Ví dụ này xử lý tương tự như một Sequence đơn giản nếu có nhiều con.
        if (Children.Count == 0)
        {
            return NodeStatus.Success; // Hoặc Failure nếu không có hành vi
        }

        if (CurrentChildIndex >= Children.Count)
        {
            Reset();
            return NodeStatus.Success;
        }

        NodeStatus childStatus = Children[CurrentChildIndex].Process();

        if (childStatus == NodeStatus.Running)
        {
            CurrentStatus = NodeStatus.Running;
            return CurrentStatus;
        }
        else if (childStatus == NodeStatus.Failure)
        {
            Reset();
            CurrentStatus = NodeStatus.Failure;
            return CurrentStatus;
        }
        else // childStatus == NodeStatus.Success
        {
            CurrentChildIndex++;
            if (CurrentChildIndex < Children.Count)
            {
                CurrentStatus = NodeStatus.Running;
                return CurrentStatus;
            }
            else
            {
                Reset();
                CurrentStatus = NodeStatus.Success;
                return CurrentStatus;
            }
        }
    }
}