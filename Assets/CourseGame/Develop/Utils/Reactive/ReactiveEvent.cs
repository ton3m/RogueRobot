using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Utils.Reactive
{
    public class ReactiveEvent
    {
        private List<ActionNode> _subscribers = new();

        public IDisposable Subscribe(Action action)
        {
            ActionNode actionNode = new ActionNode(action, Remove);
            _subscribers.Add(actionNode);
            return actionNode;
        }

        public void Invoke()
        {
            foreach(ActionNode subscriber in _subscribers)
                subscriber.Invoke();
        }

        private void Remove(ActionNode actionNode)
            => _subscribers.Remove(actionNode); 
    }

    public class ActionNode : IDisposable
    {
        private Action _action;
        private Action<ActionNode> _onDispose;

        public ActionNode(Action action, Action<ActionNode> onDispose)
        {
            _action = action;
            _onDispose = onDispose;
        }

        public void Invoke() => _action?.Invoke();
        public void Dispose() => _onDispose?.Invoke(this);
    }

    public class ReactiveEvent<T>
    {
        private List<ActionNode<T>> _subscribers = new();

        public IDisposable Subscribe(Action<T> action)
        {
            ActionNode<T> actionNode = new ActionNode<T>(action, Remove);
            _subscribers.Add(actionNode);
            return actionNode;
        }

        public void Invoke(T arg)
        {
            foreach (ActionNode<T> subscriber in _subscribers)
                subscriber.Invoke(arg);
        }

        private void Remove(ActionNode<T> actionNode)
            => _subscribers.Remove(actionNode);
    }

    public class ActionNode<T> : IDisposable
    {
        private Action<T> _action;
        private Action<ActionNode<T>> _onDispose;

        public ActionNode(Action<T> action, Action<ActionNode<T>> onDispose)
        {
            _action = action;
            _onDispose = onDispose;
        }

        public void Invoke(T arg) => _action?.Invoke(arg);
        public void Dispose() => _onDispose?.Invoke(this);
    }
}
