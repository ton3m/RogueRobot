﻿using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Entities.Behaviours;
using Assets.CourseGame.Develop.Utils.Conditions;
using Assets.CourseGame.Develop.Utils.Reactive;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementBehaviour : IEntityUpdate, IEntityInitialize
    {
        private Rigidbody _rigidbody;

        private IReadOnlyVariable<float> _speed;
        private IReadOnlyVariable<Vector3> _direction;
        private ReactiveVariable<bool> _isMoving;

        private ICondition _condition;

        public void OnInit(Entity entity)
        {
            _speed = entity.GetMoveSpeed();
            _direction = entity.GetMoveDirection();
            _rigidbody = entity.GetRigidbody();
            _condition = entity.GetMoveCondition();
            _isMoving = entity.GetIsMoving();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_condition.Evaluate() == false)
            {
                _isMoving.Value = false;
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            Vector3 velocity = _direction.Value.normalized * _speed.Value;

            _isMoving.Value = velocity.magnitude > 0;

            _rigidbody.velocity = velocity;
        }
    }
}
