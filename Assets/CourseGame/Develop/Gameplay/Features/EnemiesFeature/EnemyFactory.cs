using Assets.CourseGame.Develop.Configs.Gameplay.Creatures;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.AI;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.LootFeature;
using Assets.CourseGame.Develop.Gameplay.Features.SpawnFeature;
using Assets.CourseGame.Develop.Gameplay.Features.TeamFeature;
using Assets.CourseGame.Develop.Utils.Conditions;
using System;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Features.EnemiesFeature
{
    public class EnemyFactory
    {
        private EntityFactory _entityFactory;
        private AIFactory _aiFactory;
        private readonly int _team = TeamTypes.Enemies;

        private EntitiesBuffer _eneitiesBuffer;
        private DropLootService _dropLootService;

        public EnemyFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntityFactory>();
            _aiFactory = container.Resolve<AIFactory>();
            _eneitiesBuffer = container.Resolve<EntitiesBuffer>();
            _dropLootService = container.Resolve<DropLootService>();
        }

        public Entity Create(Vector3 position, CreatureConfig config)
        {
            Entity entity;
            AIStateMachine brain;

            switch (config)
            {
                case GhostConfig ghostConfig:
                    entity = _entityFactory.CreateGhost(position, ghostConfig, _team);
                    brain = _aiFactory.CreateGhostBehaviour(entity);
                    break;

                default:
                    throw new ArgumentException("Не поддерживается такой конфиг для создания врага");
            }

            entity.AddBehaviour(new StateMachineBrainBehaviour(brain));

            AddDropLootBehaviourTo(entity);

            _eneitiesBuffer.Add(entity);

            return entity;
        }

        private void AddDropLootBehaviourTo(Entity entity)
        {
            ICompositeCondition dropLootCondition = new CompositeCondition(LogicOperations.AndOperation)
                .Add(new FuncCondition(() => entity.GetIsDead().Value))
                .Add(new FuncCondition(() => entity.GetLootIsDropped().Value == false));

            entity
                .AddLootIsDropped()
                .AddDropLootCondition(dropLootCondition);

            entity.GetSelfDestroyCondition()
                .Add(new FuncCondition(() => entity.GetLootIsDropped().Value));

            entity.AddBehaviour(new DropLootBehaviour(_dropLootService));
        }
    }
}
