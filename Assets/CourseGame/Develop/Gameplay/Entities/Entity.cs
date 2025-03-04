﻿using Assets.CourseGame.Develop.Gameplay.Entities.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Entities
{
    public class Entity : MonoBehaviour, IEquatable<Entity>
    {
        public event Action<Entity> Initialized;
        public event Action<Entity> Disposed;

        private readonly Dictionary<EntityValues, object> _values = new();

        private readonly HashSet<IEntityBehaviour> _behaviours = new();

        private readonly List<IEntityUpdate> _updatables = new();
        private readonly List<IEntityInitialize> _initializables = new();
        private readonly List<IEntityDispose> _disposeables = new();

        private bool _isInit;

        private void Awake()
        {
            Install();
        }

        private void Install()
        {
            MonoEntityRegistrator[] registrators = GetComponents<MonoEntityRegistrator>();

            if (registrators != null)
                foreach (MonoEntityRegistrator registrator in registrators)
                    registrator.Register(this);
        }

        public void Initialize()
        {
            foreach (IEntityInitialize initializable in _initializables)
                initializable.OnInit(this);

            _isInit = true;
            Initialized?.Invoke(this);
        }

        private void Update()
        {
            if (_isInit == false)
                throw new InvalidOperationException("update for not inited");

            foreach (IEntityUpdate updatable in _updatables)
                updatable.OnUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            foreach (IEntityDispose disposable in _disposeables)
                disposable.OnDispose();

            Disposed?.Invoke(this);
        }

        public Entity AddValue<TValue>(EntityValues valueType, TValue value)
        {
            if (_values.ContainsKey(valueType))
                throw new ArgumentException(valueType.ToString());

            _values.Add(valueType, value);
            return this;
        }

        public bool TryGetValue<TValue>(EntityValues valueType, out TValue value)
        {
            if(_values.TryGetValue(valueType, out object findedObject))
            {
                if(findedObject is TValue findedValue)
                {
                    value = findedValue;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        public TValue GetValue<TValue>(EntityValues valueType)
        {
            if(TryGetValue(valueType, out TValue value) == false)
                throw new ArgumentException($"Entity not exist {valueType}");

            return value;
        }

        public Entity AddBehaviour(IEntityBehaviour behaviour)
        {
            if(_behaviours.Contains(behaviour))
                throw new ArgumentException(behaviour.GetType().ToString());    

            _behaviours.Add(behaviour);

            if(behaviour is IEntityUpdate updatable)
                _updatables.Add(updatable);

            if(behaviour is IEntityInitialize initializable)
            {
                _initializables.Add(initializable);

                if (_isInit)
                    initializable.OnInit(this);
            }

            if(behaviour is IEntityDispose disposable) 
                _disposeables.Add(disposable);

            return this;
        }

        public bool TryRemoveBehaviour<T>() where T : IEntityBehaviour
        {
            IEntityBehaviour entityBehaviour = _behaviours.FirstOrDefault(beh => beh is T);

            if (entityBehaviour == null)
                return false;

            _behaviours.Remove(entityBehaviour);

            if (entityBehaviour is IEntityUpdate updatable)
                _updatables.Remove(updatable);

            if (entityBehaviour is IEntityInitialize initializable)
                _initializables.Remove(initializable);

            if (entityBehaviour is IEntityDispose disposable)
            {
                disposable.OnDispose();
                _disposeables.Remove(disposable);
            }

            return true;
        }

        public bool Equals(Entity other) => other == this;
    }
}
