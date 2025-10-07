using System;
using System.Collections.Generic;
using System.Linq; // Thêm để sử dụng OrderByDescending

public enum NodeStatus
{
    Running,
    Success,
    Failure
}

public abstract class Node
{
    public string Name { get; protected set; }
    protected NodeStatus CurrentStatus { get; set; }
    protected List<Node> Children { get; set; } = new List<Node>();
    protected int CurrentChildIndex { get; set; } = 0;
    public int Priority { get; protected set; } = 0;

    public Node(string name = "Node", int priority = 0)
    {
        Name = name;
        Priority = priority;
    }

    public void AddChild(Node child)
    {
        Children.Add(child);
    }

    // Thêm phương thức để sắp xếp Children theo Priority
    public void SortChildrenByPriority()
    {
        Children = Children.OrderByDescending(c => c.Priority).ToList();
    }

    public virtual NodeStatus Process()
    {
        if (Children.Count > 0)
        {
            CurrentStatus = Children[CurrentChildIndex].Process();
            return CurrentStatus;
        }
        return NodeStatus.Failure;
    }

    public virtual void Reset()
    {
        CurrentChildIndex = 0;
        foreach (var child in Children)
        {
            child.Reset();
        }
    }
}

public interface IStrategy
{
    NodeStatus Process();
    void Reset();
}

public class Leaf : Node
{
    private IStrategy _strategy;

    public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
    {
        _strategy = strategy;
    }

    public override NodeStatus Process()
    {
        CurrentStatus = _strategy.Process();
        return CurrentStatus;
    }

    public override void Reset()
    {
        base.Reset();
        _strategy.Reset();
    }
}

public class ConditionStrategy : IStrategy
{
    private Func<bool> _condition;

    public ConditionStrategy(Func<bool> condition)
    {
        _condition = condition;
    }

    public NodeStatus Process()
    {
        if (_condition())
        {
            return NodeStatus.Success;
        }
        else
        {
            return NodeStatus.Failure;
        }
    }

    public void Reset()
    {
        // Không cần đặt lại trạng thái cho điều kiện đơn giản
    }
}

public class ActionStrategy : IStrategy
{
    private Action _action;

    public ActionStrategy(Action action)
    {
        _action = action;
    }

    public NodeStatus Process()
    {
        _action();
        return NodeStatus.Success;
    }

    public void Reset()
    {
        // Không cần đặt lại trạng thái cho hành động đơn giản
    }
}