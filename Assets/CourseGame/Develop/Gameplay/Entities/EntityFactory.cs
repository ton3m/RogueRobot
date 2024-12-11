using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Features.MovementFeature;
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
                .AddRotationSpeed(new ReactiveVariable<float>(900));

            instance
                .AddBehaviour(new CharacterControllerMovementBehaviour())
                .AddBehaviour(new RotationBehaviour());

            instance.Initialize();

            return instance;
        }
    }
}
