using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.AI;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.TeamFeature;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Features.EnemiesFeature
{
    public class EnemyFactory
    {
        private EntityFactory _entityFactory;
        private AIFactory _aiFactory;
        private readonly int _team = TeamTypes.Enemies;

        private EntitiesBuffer _eneitiesBuffer;

        public EnemyFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntityFactory>();
            _aiFactory = container.Resolve<AIFactory>();
            _eneitiesBuffer = container.Resolve<EntitiesBuffer>();
        }

        public Entity CreateGhost(Vector3 position)
        {
            Entity entity = _entityFactory.CreateGhost(position, _team);
            AIStateMachine brain = _aiFactory.CreateGhostBehaviour(entity);

            entity.AddBehaviour(new StateMachineBrainBehaviour(brain));

            _eneitiesBuffer.Add(entity);

            return entity;
        }
    }
}
