﻿using Assets.CourseGame.Develop.Utils.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.CourseGame.Develop.Utils.StateMachineBase
{
    public abstract class StateMachine<TState> : IDisposable where TState : class, IState
    {
        private List<StateNode<TState>> _states = new();

        private StateNode<TState> _currentState;

        private bool _isRunning;

        private List<IDisposable> _disposables;

        protected StateMachine(List<IDisposable> disposables = null)
        {
            if (disposables == null)
                return;

            _disposables = new List<IDisposable>(disposables);
        }

        public void AddState(TState state) => _states.Add(new StateNode<TState>(state));

        public void AddTransition(TState fromState, TState toState, ICondition condition)
        {
            StateNode<TState> from = _states.First(stateNode => stateNode.State == fromState);
            StateNode<TState> to = _states.First(stateNode => stateNode.State == toState);

            from.AddTransition(new StateTransition<TState>(to, condition));
        }

        protected StateNode<TState> CurrentState => _currentState;

        public void Enter()
        {
            if (_currentState == null)
                SwitchState(_states[0]);

            _isRunning = true;
        }

        public void Exit()
        {
            _currentState?.State.Exit();

            _isRunning = false;
        }

        public virtual void Update(float deltaTime)
        {
            if(_isRunning == false)
                return;

            StateTransition<TState> stateTransition = _currentState.Transitions
                .Where(transition => transition.Condition.Evaluate())
                .FirstOrDefault();

            if (stateTransition != null)
                SwitchState(stateTransition.ToState);
        }

        private void SwitchState(StateNode<TState> stateNode)
        {
            _currentState?.State.Exit();
            _currentState = stateNode;
            _currentState.State.Enter();
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();   

            _disposables.Clear();
        }
    }
}
