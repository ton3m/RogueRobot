using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Features.DamageFeature;
using Assets.CourseGame.Develop.Gameplay.Features.DeathFeature;
using Assets.CourseGame.Develop.Gameplay.Features.MovementFeature;
using Assets.CourseGame.Develop.Utils.Conditions;
using Assets.CourseGame.Develop.Utils.Reactive;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Entities
{
    public class EntityFactory
    {
        private string GhostPrefabPath = "Gameplay/Creatures/Ghost";

        private DIContainer _container;
        private ResourcesAssetLoader _assets;

        public EntityFactory(DIContainer container)
        {
            _container = container;
            _assets = container.Resolve<ResourcesAssetLoader>();
        }

        public Entity CreateGhost(Vector3 position)
        {
            Entity prefab = _assets.LoadResource<Entity>(GhostPrefabPath);

            Entity instance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            instance
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(10))
                .AddRotationDirection()
                .AddRotationSpeed(new ReactiveVariable<float>(900))
                .AddHealth(new ReactiveVariable<float>(400))
                .AddMaxHealth(new ReactiveVariable<float>(400))
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddIsDead();

            ICompositeCondition deathCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetHealth().Value <= 0));

            ICompositeCondition takeDamageCondition = new CompositeCondition(LogicOperations.AndOperation)
               .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition moveCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition rotationCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value == false));

            ICompositeCondition selfDestroyCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => instance.GetIsDead().Value));

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
                .AddBehaviour(new DeathBehaviour())
                .AddBehaviour(new SelfDestroyBehaviour());

            instance.Initialize();

            return instance;
        }
    }
}
