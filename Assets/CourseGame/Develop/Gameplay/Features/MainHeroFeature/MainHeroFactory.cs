using Assets.CourseGame.Develop.CommonServices.ConfigsManagment;
using Assets.CourseGame.Develop.Configs.Gameplay.Creatures;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.AI;
using Assets.CourseGame.Develop.Gameplay.AI.Sensors;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.AbilitiesFeature;
using Assets.CourseGame.Develop.Gameplay.Features.TeamFeature;
using Assets.CourseGame.Develop.Utils.Reactive;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Features.MainHeroFeature
{
    public class MainHeroFactory
    {
        private EntityFactory _entityFactory;
        private AIFactory _aiFactory;
        private AbilityFactory _abilityFactory;
        private ConfigsProviderService _configsProviderService;
        private MainHeroHolderService _heroHolder;

        private readonly int _team = TeamTypes.MainHero;

        private EntitiesBuffer _eneitiesBuffer;

        public MainHeroFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntityFactory>();
            _eneitiesBuffer = container.Resolve<EntitiesBuffer>();
            _aiFactory = container.Resolve<AIFactory>();
            _heroHolder = container.Resolve<MainHeroHolderService>();
            _abilityFactory = container.Resolve<AbilityFactory>();
            _configsProviderService = container.Resolve<ConfigsProviderService>();
        }

        public Entity Create(Vector3 position, MainHeroConfig config)
        {
            Entity entity = _entityFactory.CreateMainHero(position, config, _team);
            AIStateMachine brain = _aiFactory.CreateMainHeroBehaviour(entity, new NearestDamageableTargetSelector(entity.GetTransform(), entity.GetTeam()));

            AbilityList abilityList = new AbilityList();
            abilityList.Add(_abilityFactory.CreateAbilityFor(entity, _configsProviderService.AbilitiesConfigsContaier.AbilityConfigs[0]));

            entity
                .AddIsMainHero(new ReactiveVariable<bool>(true))
                .AddAbilityList(abilityList);

            entity
                .AddBehaviour(new StateMachineBrainBehaviour(brain))
                .AddBehaviour(new AbilityOnAddActivatorBehaviour());

            _heroHolder.Register(entity);
            _eneitiesBuffer.Add(entity);

            return entity;
        }
    }
}
