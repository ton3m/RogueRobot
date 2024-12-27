﻿using Assets.CourseGame.Develop.Configs.Gameplay.Levels.WaveStage;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.EnemiesFeature;
using Assets.CourseGame.Develop.Utils.Reactive;
using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Gameplay.Features.GameModeStagesFeature.WaveGameModeFeature
{
    public class WaveGameMode
    {
        private ReactiveEvent _ended = new();

        private EnemyFactory _enemyFactory;

        private List<EntityToRemoveReason> _currentSpawnedEnemies = new();

        private ReactiveVariable<bool> _inProcess = new();

        public WaveGameMode(EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public IReadOnlyVariable<bool> InProcess => _inProcess;
        public IReadOnlyEvent Ended => _ended;

        public void Start(WaveConfig waveConfig)
        {
            if (InProcess.Value)
                throw new InvalidOperationException("is already started");

            SetupWave(waveConfig);

            _inProcess.Value = true;
        }

        public void Cleanup()
        {
            foreach (EntityToRemoveReason entityToRemoveReason in _currentSpawnedEnemies)
            {
                entityToRemoveReason.Dispose();
                UnityEngine.Object.Destroy(entityToRemoveReason.Entity.gameObject);
            }

            _currentSpawnedEnemies.Clear();

            _inProcess.Value = false;
        }

        private void SetupWave(WaveConfig waveConfig)
        {
            foreach (WaveItemConfig waveItemConfig in waveConfig.WaveItems)
                SpawnEnemy(waveItemConfig);
        }

        private void SpawnEnemy(WaveItemConfig waveItemConfig)
        {
            Entity spawnedEnemy = _enemyFactory.Create(waveItemConfig.SpawnPosition, waveItemConfig.EnemyConfig);
            EntityToRemoveReason entityToRemoveReason = new EntityToRemoveReason(spawnedEnemy);
            entityToRemoveReason.OnRemoveReasonComplete += OnEnemyRemoved;
            _currentSpawnedEnemies.Add(entityToRemoveReason);   
        }

        private void OnEnemyRemoved(EntityToRemoveReason entityToRemoveReason)
        {
            entityToRemoveReason.OnRemoveReasonComplete -= OnEnemyRemoved;

            if (_currentSpawnedEnemies.Contains(entityToRemoveReason))
            {
                _currentSpawnedEnemies.Remove(entityToRemoveReason);

                if (_currentSpawnedEnemies.Count == 0)
                    _ended?.Invoke();
            }
            else
            {
                throw new InvalidOperationException("entityToRemoveReason not exist in list");
            }
        }

        private class EntityToRemoveReason : IDisposable
        {
            public event Action<EntityToRemoveReason> OnRemoveReasonComplete;

            public EntityToRemoveReason(Entity entity)
            {
                Entity = entity;
                Entity.GetIsDead().Changed += OnEntityDied;
            }

            public Entity Entity { get; private set; }

            private void OnEntityDied(bool arg1, bool isDead)
            {
                if (isDead)
                {
                    Entity.GetIsDead().Changed -= OnEntityDied;
                    OnRemoveReasonComplete?.Invoke(this);
                }
            }

            public void Dispose()
            {
                Entity.GetIsDead().Changed -= OnEntityDied;
            }
        }
    }
}
