﻿using Assets.CourseGame.Develop.Gameplay.Features.InputFeature;
using Assets.CourseGame.Develop.Utils.Reactive;
using Assets.CourseGame.Develop.Utils.StateMachineBase;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.AI.States
{
    public class PlayerDirectionGenerateState : State, IUpdatableState
    {
        private IInputService _inputService;
        private ReactiveVariable<Vector3> _movementDirection;
        private ReactiveVariable<Vector3> _rotationDirection;

        public PlayerDirectionGenerateState(IInputService inputService, ReactiveVariable<Vector3> movementDireciton, ReactiveVariable<Vector3> rotationDirection)
        {
            _inputService = inputService;
            _movementDirection = movementDireciton;
            _rotationDirection = rotationDirection;
        }

        public void Update(float deltaTime)
        {
            _movementDirection.Value = _inputService.Direction;
            _rotationDirection.Value = _movementDirection.Value;    
        }

        public override void Exit()
        {
            base.Exit();

            _movementDirection.Value = Vector3.zero;
        }
    }
}
