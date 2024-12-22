﻿using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Features.AttackFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DamageFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DeathFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DetecteBufferFeautre;
using Assets.CourseGame.Develop.Gameplay.Features.MovementFeature;
using Assets.CourseGame.Develop.Utils.Conditions;
using Assets.CourseGame.Develop.Utils.Reactive;
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

        public Entity CreateMainHero(Vector3 position, int team)
        {
            Entity prefab = _assets.LoadResource<Entity>(MainHeroPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            instance
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(10))
                .AddIsMoving()
                .AddRotationDirection()
                .AddRotationSpeed(new ReactiveVariable<float>(900))
                .AddHealth(new ReactiveVariable<float>(800))
                .AddMaxHealth(new ReactiveVariable<float>(800))
                .AddDamage(new ReactiveVariable<float>(100))
                .AddIntervalBetweenAttacks(new ReactiveVariable<float>(4f))
                .AddAttackCooldown()
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddDetectedEntitiesBuffer()
                .AddAttackTrigger()
                .AddIsAttackProcess()
                .AddInstantAttackEvent()
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
                .AddBehaviour(new UpdateEntityBufferFromCreaturesBuffer(_entitiesBuffer))
                .AddBehaviour(new CharacterControllerMovementBehaviour())
                .AddBehaviour(new RotationBehaviour())
                .AddBehaviour(new ApplyDamageFilterBehaviour())
                .AddBehaviour(new ApplyDamageBehaviour())
                .AddBehaviour(new AttackCooldownProcessBehaviour())
                .AddBehaviour(new RestartAttackCooldownOnInstantAttackBehaviour())
                .AddBehaviour(new EndAttackCooldownOnMovingBehaviour())
                .AddBehaviour(new AttackBehaviour())
                .AddBehaviour(new AttackCancelBehaviour())
                .AddBehaviour(new InstantShootBehaviour(this))
                .AddBehaviour(new DeathBehaviour())
                .AddBehaviour(new SelfDestroyBehaviour());

            instance.Initialize();

            return instance;
        }

        public Entity CreateGhost(Vector3 position, int team)
        {
            Entity prefab = _assets.LoadResource<Entity>(GhostPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            instance
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(10))
                .AddIsMoving()
                .AddRotationDirection()
                .AddRotationSpeed(new ReactiveVariable<float>(900))
                .AddHealth(new ReactiveVariable<float>(800))
                .AddMaxHealth(new ReactiveVariable<float>(800))
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddIsDead()
                .AddIsDeathProcess()
                .AddSelfTriggerDamage(new ReactiveVariable<float>(150))
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
    }
}
