using Assets.CourseGame.Develop.CommonServices.ConfigsManagment;
using Assets.CourseGame.Develop.Configs.Gameplay.Creatures;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.AI;
using Assets.CourseGame.Develop.Gameplay.AI.Sensors;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.AbilitiesFeature;
using Assets.CourseGame.Develop.Gameplay.Features.LevelUPFeature;
using Assets.CourseGame.Develop.Gameplay.Features.StatsFeature;
using Assets.CourseGame.Develop.Gameplay.Features.TeamFeature;
using Assets.CourseGame.Develop.Utils.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Features.MainHeroFeature
{
    public class MainHeroFactory
    {
        private EntityFactory _entityFactory;
        private AIFactory _aiFactory;
        private ConfigsProviderService _configsProviderService;
        private MainHeroHolderService _heroHolder;
        private StatsUpgradeService _statsUpgradeService;

        private readonly int _team = TeamTypes.MainHero;

        private EntitiesBuffer _eneitiesBuffer;

        public MainHeroFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntityFactory>();
            _eneitiesBuffer = container.Resolve<EntitiesBuffer>();
            _aiFactory = container.Resolve<AIFactory>();
            _heroHolder = container.Resolve<MainHeroHolderService>();
            _configsProviderService = container.Resolve<ConfigsProviderService>();
            _statsUpgradeService = container.Resolve<StatsUpgradeService>();
        }

        public Entity Create(Vector3 position, MainHeroConfig config)
        {
            Entity entity = _entityFactory.CreateMainHero(position, GetStats(), config, _team);
            AIStateMachine brain = _aiFactory.CreateMainHeroBehaviour(entity, new NearestDamageableTargetSelector(entity.GetTransform(), entity.GetTeam()));

            entity
                .AddIsMainHero(new ReactiveVariable<bool>(true))
                .AddLevel(new ReactiveVariable<int>(1))
                .AddExperience()
                .AddCoins()
                .AddAbilityList();

            entity
                .AddBehaviour(new StateMachineBrainBehaviour(brain))
                .AddBehaviour(new LevelUpBehaviour(_configsProviderService.ExperienceForUpgradeLevelConfig))
                .AddBehaviour(new AbilityOnAddActivatorBehaviour());

            _heroHolder.Register(entity);
            _eneitiesBuffer.Add(entity);

            return entity;
        }

        private Dictionary<StatTypes, float> GetStats()
        {
            Dictionary<StatTypes, float> stats = new();

            foreach (StatTypes statType in Enum.GetValues(typeof(StatTypes)))
                stats.Add(statType, _statsUpgradeService.GetCurrentStatValueFor(statType));

            return stats;
        }
    }
}
