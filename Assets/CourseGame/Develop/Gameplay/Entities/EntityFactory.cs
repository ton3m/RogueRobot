﻿using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.Configs.Gameplay.Creatures;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Features.AttackFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DamageFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DeathFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DetecteBufferFeautre;
using Assets.CourseGame.Develop.Gameplay.Features.MovementFeature;
using Assets.CourseGame.Develop.Gameplay.Features.StatsFeature;
using Assets.CourseGame.Develop.Utils.Conditions;
using Assets.CourseGame.Develop.Utils.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Entities
{
    public class EntityFactory
    {
        private string GhostPrefabPath = "Gameplay/Creatures/Ghost";
        private string MainHeroPrefabPath = "Gameplay/Creatures/MainHero";

        private DIContainer _container;
        private ResourcesAssetLoader _assets;
        private EntitiesBuffer _entitiesBuffer;

        public EntityFactory(DIContainer container)
        {
            _container = container;
            _assets = container.Resolve<ResourcesAssetLoader>();
            _entitiesBuffer = container.Resolve<EntitiesBuffer>();
        }

        public Entity CreateMainHero(Vector3 position, MainHeroConfig config, int team)
        {
            Entity prefab = _assets.LoadResource<Entity>(MainHeroPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            Dictionary<StatTypes, float> baseStats = new Dictionary<StatTypes, float>()
            {
                {StatTypes.MoveSpeed, config.MoveSpeed},
                {StatTypes.AttackInterval, config.AttackInterval},
                {StatTypes.MaxHealth, config.MaxHealth},
                {StatTypes.Damage, config.Damage},
            };

            Dictionary<StatTypes, float> modifiedStats = new Dictionary<StatTypes, float>(baseStats);

            //StatsEffectsList statsEffectsList = new StatsEffectsList();
            //statsEffectsList.Add(new StatsEffect(StatTypes.MoveSpeed, stat => stat *= 0.3f));

            instance
                .AddStatsEffectsList()
                .AddBaseStats(baseStats)
                .AddModifiedStats(modifiedStats)
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(baseStats[StatTypes.MoveSpeed]))
                .AddIsMoving()
                .AddRotationDirection()
                .AddRotationSpeed(new ReactiveVariable<float>(config.RotationSpeed))
                .AddHealth(new ReactiveVariable<float>(baseStats[StatTypes.MaxHealth]))
                .AddMaxHealth(new ReactiveVariable<float>(baseStats[StatTypes.MaxHealth]))
                .AddDamage(new ReactiveVariable<float>(baseStats[StatTypes.Damage]))
                .AddIntervalBetweenAttacks(new ReactiveVariable<float>(baseStats[StatTypes.AttackInterval]))
                .AddAttackCooldown()
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddDetectedEntitiesBuffer()
                .AddAttackTrigger()
                .AddIsAttackProcess()
                .AddInstantAttackEvent()
                .AddInstanShootingDirections(new InstantShootingDirectionArgs(new InstantShotDirectionArgs(0, 1)))
                .AddIsDead()
                .AddIsDeathProcess()
                .AddTeam(new ReactiveVariable<int>(team));

            ICompositeCondition attackCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false))
                .Add(new FuncCondition(() => instance.GetIsMoving().Value == false))
                .Add(new FuncCondition(() => instance.GetAttackCooldown().Value <= 0))
                .Add(new FuncCondition(() => instance.GetIsAttackProcess().Value == false));

            ICompositeCondition attackProcessCanceledCondition = new CompositeCondition(LogicOperations.OrOperation)
                .Add(new FuncCondition(() => instance.GetIsMoving().Value))
                .Add(new FuncCondition(() => instance.GetIsDead().Value));

            ICompositeCondition deathCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetHealth().Value <= 0));

            ICompositeCondition takeDamageCondition = new CompositeCondition(LogicOperations.AndOperation)
               .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition moveCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition rotationCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition selfDestroyCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value))
                .Add(new FuncCondition(() => instance.GetIsDeathProcess().Value == false));

            instance
                .AddMoveCondition(moveCondition)
                .AddRotationCondition(rotationCondition)
                .AddDeathCondition(deathCondition)
                .AddTakeDamageCondition(takeDamageCondition)
                .AddSelfDestroyCondition(selfDestroyCondition)
                .AddAttackCanceledCondition(attackProcessCanceledCondition)
                .AddAttackCondition(attackCondition);

            instance
                .AddBehaviour(new StatEffectsApplierBehaviour())
                .AddBehaviour(new MoveSpeedModifierApplierBehaviour())
                .AddBehaviour(new DamageModifierApplierBehaviour())
                .AddBehaviour(new MaxHealthModifierApplierBehaviour())
                .AddBehaviour(new AttackIntervalModifierApplierBehaviour())
                .AddBehaviour(new UpdateEntityBufferFromCreaturesBuffer(_entitiesBuffer))
                .AddBehaviour(new CharacterControllerMovementBehaviour())
                .AddBehaviour(new RotationBehaviour())
                .AddBehaviour(new ApplyDamageFilterBehaviour())
                .AddBehaviour(new ApplyDamageBehaviour())
                .AddBehaviour(new AttackCooldownProcessBehaviour())
                .AddBehaviour(new RestartAttackCooldownOnInstantAttackBehaviour())
                .AddBehaviour(new EndAttackCooldownOnMovingBehaviour())
                .AddBehaviour(new AttackBehaviour())
                .AddBehaviour(new DirectionsInstantShootingBehaviour(this))
                .AddBehaviour(new AttackCancelBehaviour())
                .AddBehaviour(new DeathBehaviour())
                .AddBehaviour(new SelfDestroyBehaviour());

            instance.Initialize();

            return instance;
        }

        public Entity CreateGhost(Vector3 position, GhostConfig config, int team)
        {
            Entity prefab = _assets.LoadResource<Entity>(GhostPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            instance
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
                .AddIsMoving()
                .AddRotationDirection()
                .AddRotationSpeed(new ReactiveVariable<float>(config.RotationSpeed))
                .AddHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddIsDead()
                .AddIsDeathProcess()
                .AddSelfTriggerDamage(new ReactiveVariable<float>(config.SelfTriggerDamage))
                .AddTeam(new ReactiveVariable<int>(team));

            ICompositeCondition deathCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetHealth().Value <= 0));

            ICompositeCondition takeDamageCondition = new CompositeCondition(LogicOperations.AndOperation)
               .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition moveCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition rotationCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition selfDestroyCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value))
                .Add(new FuncCondition(() => instance.GetIsDeathProcess().Value == false));

            instance
                .AddMoveCondition(moveCondition)
                .AddRotationCondition(rotationCondition)
                .AddDeathCondition(deathCondition)
                .AddTakeDamageCondition(takeDamageCondition)
                .AddSelfDestroyCondition(selfDestroyCondition);

            instance
                .AddBehaviour(new CharacterControllerMovementBehaviour())
                .AddBehaviour(new RotationBehaviour())
                .AddBehaviour(new ApplyDamageFilterBehaviour())
                .AddBehaviour(new ApplyDamageBehaviour())
                .AddBehaviour(new DealDamageOnSelfTriggerBehaviour())
                .AddBehaviour(new DeathBehaviour())
                .AddBehaviour(new SelfDestroyBehaviour());

            instance.Initialize();

            return instance;
        }


        //снаряды

        private string ArrowPrefabPath = "Gameplay/Projectiles/Arrow";

        public Entity CreateArrow(Vector3 position, Vector3 direction, float damage)
        {
            Entity prefab = _assets.LoadResource<Entity>(ArrowPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            instance
                .AddMoveDirection(new ReactiveVariable<Vector3>(direction))
                .AddRotationDirection(new ReactiveVariable<Vector3>(direction))
                .AddMoveSpeed(new ReactiveVariable<float>(20))
                .AddIsMoving();

            ICompositeCondition moveCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => true));

            ICompositeCondition rotationCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => true));

            instance
                .AddMoveCondition(moveCondition)
                .AddRotationCondition(rotationCondition);

            instance
                .AddBehaviour(new RigidbodyMovementBehaviour())
                .AddBehaviour(new ForceRotationBehaviour());

            instance.Initialize();

            _entitiesBuffer.Add(instance);

            return instance;
        }

        // Геймплейные штучки

        private const string NextStageTriggerPrefabPath = "Gameplay/NextGameplayStageTrigger";

        public Entity CreateNextGameplayStageTrigger(Vector3 position)
        {
            Entity prefab = _assets.LoadResource<Entity>(NextStageTriggerPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            instance.Initialize();

            _entitiesBuffer.Add(instance);

            return instance;
        }
    }
}
