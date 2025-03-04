﻿using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Utils.Reactive
{
    public class ReactiveEvent : IReadOnlyEvent
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

    public class ReactiveEvent<T> : IReadOnlyEvent<T>
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
}
