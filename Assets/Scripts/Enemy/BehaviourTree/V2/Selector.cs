using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Thêm để sử dụng OrderByDescending

public class Selector : Node
{
    public Selector(string name, int priority = 0) : base(name, priority)
    {
    }

    public override NodeStatus Process()
    {
        // Sắp xếp children theo priority descending mỗi lần process để đảm bảo ưu tiên cao chạy trước
        Children = Children.OrderByDescending(c => c.Priority).ToList();

        if (CurrentChildIndex >= Children.Count)
        {
            Reset();
            return NodeStatus.Failure; // Tất cả các con đều thất bại
        }

        NodeStatus childStatus = Children[CurrentChildIndex].Process();

        if (childStatus == NodeStatus.Running)
        {
            CurrentStatus = NodeStatus.Running;
            return CurrentStatus;
        }
        else if (childStatus == NodeStatus.Success)
        {
            Reset();
            CurrentStatus = NodeStatus.Success; // Một con thành công
            return CurrentStatus;
        }
        else // childStatus == NodeStatus.Failure
        {
            CurrentChildIndex++;
            if (CurrentChildIndex < Children.Count)
            {
                CurrentStatus = NodeStatus.Running; // Thử nút con tiếp theo
                return CurrentStatus;
            }
            else
            {
                Reset();
                CurrentStatus = NodeStatus.Failure; // Tất cả các con đều thất bại
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