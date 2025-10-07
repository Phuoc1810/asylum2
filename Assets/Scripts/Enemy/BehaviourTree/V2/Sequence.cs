using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name, int priority = 0) : base(name, priority)
    {
    }

    public override NodeStatus Process()
    {
        if (CurrentChildIndex >= Children.Count)
        {
            Reset(); // Đặt lại nếu đã xử lý hết các con
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
                CurrentStatus = NodeStatus.Running; // Tiếp tục chạy nếu còn con
                return CurrentStatus;
            }
            else
            {
                Reset();
                CurrentStatus = NodeStatus.Success; // Tất cả các con đều thành công
                return CurrentStatus;
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        CurrentChildIndex = 0;
    }
}