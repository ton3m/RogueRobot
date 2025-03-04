﻿using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Utils.StateMachineBase
{
    public class StateNode<TState> where TState : IState
    {
        private List<StateTransition<TState>> _transitions = new();

        public StateNode(TState state)
        {
            State = state;
        }

        public TState State { get; }

        public IReadOnlyList<StateTransition<TState>> Transitions => _transitions;

        public void AddTransition(StateTransition<TState> transition) => _transitions.Add(transition);
    }
}
