using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.AI;
using Assets.CourseGame.Develop.Gameplay.AI.Sensors;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.TeamFeature;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Features.MainHeroFeature
{
    public class MainHeroFactory
    {
        private EntityFactory _entityFactory;
        private AIFactory _aiFactory;

        private readonly int _team = TeamTypes.MainHero;

        private EntitiesBuffer _eneitiesBuffer;

        public MainHeroFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntityFactory>();
            _eneitiesBuffer = container.Resolve<EntitiesBuffer>();
            _aiFactory = container.Resolve<AIFactory>();
        }

        public Entity Create(Vector3 position)
        {
            Entity entity = _entityFactory.CreateMainHero(position, _team);
            AIStateMachine brain = _aiFactory.CreateMainHeroBehaviour(entity, new NearestDamageableTargetSelector(entity.GetTransform(), entity.GetTeam()));

            entity.AddBehaviour(new StateMachineBrainBehaviour(brain));
            _eneitiesBuffer.Add(entity);

            return entity;
        }
    }
}
