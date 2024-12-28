using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.CommonServices.ConfigsManagment;
using Assets.CourseGame.Develop.CommonServices.CoroutinePerfomer;
using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.AI;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.AbilitiesFeature;
using Assets.CourseGame.Develop.Gameplay.Features.AbilitiesFeature.AbilityDropServiceFeature;
using Assets.CourseGame.Develop.Gameplay.Features.AbilitiesFeature.Presenters;
using Assets.CourseGame.Develop.Gameplay.Features.EnemiesFeature;
using Assets.CourseGame.Develop.Gameplay.Features.GameModeStagesFeature;
using Assets.CourseGame.Develop.Gameplay.Features.InputFeature;
using Assets.CourseGame.Develop.Gameplay.Features.LevelUPFeature;
using Assets.CourseGame.Develop.Gameplay.Features.MainHeroFeature;
using Assets.CourseGame.Develop.Gameplay.Features.PauseFeature;
using Assets.CourseGame.Develop.Gameplay.Features.TeamFeature;
using Assets.CourseGame.Develop.Gameplay.States;
using Assets.CourseGame.Develop.Gameplay.UI;
using Assets.CourseGame.Develop.Utils.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Infrastructure
{
    public class GameplayBootstrap : MonoBehaviour
    {
        private DIContainer _container;

        private GameplayStateMachine _gameplayStateMachine;

        public IEnumerator Run(DIContainer container, GameplayInputArgs gameplayInputArgs)
        {
            _container = container;

            ProcessRegistrations(gameplayInputArgs);

            Debug.Log($"���������� ������� ��� ������ {gameplayInputArgs.LevelNumber}");
            Debug.Log("������� ���������");
            Debug.Log("����� ������ ����� �������� ����");

            yield return new WaitForSeconds(1f);

            _gameplayStateMachine = CreateGameplayStateMachine(gameplayInputArgs);
            _gameplayStateMachine.Enter();
        }

        private void ProcessRegistrations(GameplayInputArgs gameplayInputArgs)
        {
            //������ ����������� ��� ����� ��������

            _container.RegisterAsSingle<IInputService>(c => new DesktopInput());
            _container.RegisterAsSingle<IPauseService>(c => new TimeScalePauseService());

            _container.RegisterAsSingle(c => new EntitiesBuffer());

            _container.RegisterAsSingle(c => new EntityFactory(c));
            _container.RegisterAsSingle(c => new AIFactory(c));
            _container.RegisterAsSingle(c => new EnemyFactory(c));

            _container.RegisterAsSingle(c => new AbilityFactory(c));
            _container.RegisterAsSingle(c => new AbilityPresentersFactory(c));
            _container.RegisterAsSingle(c => new DropAbilityOnMainHeroLevelUpService(
                c.Resolve<MainHeroHolderService>(), c.Resolve<AbilityPresentersFactory>(),
                c.Resolve<IPauseService>(),c.Resolve<ICoroutinePerformer>())).NonLazy();
            _container.RegisterAsSingle(c => new AbilityDropService(
                c.Resolve<ConfigsProviderService>().LevelsListConfig.GetBy(gameplayInputArgs.LevelNumber).AbilityDropOptions,
                new AbilityDropingRules()));

            _container.RegisterAsSingle(c => new MainHeroFactory(c));
            _container.RegisterAsSingle(c => new MainHeroHolderService());
            _container.RegisterAsSingle(c => new MainHeroFinishConditionCreator(
                c.Resolve<MainHeroHolderService>(), c.Resolve<GameplayFinishConditionService>())).NonLazy();

            _container.RegisterAsSingle(c => new GameModesFactory(c));
            _container.RegisterAsSingle(c => new StageProviderService(
                c.Resolve<ConfigsProviderService>().LevelsListConfig.GetBy(gameplayInputArgs.LevelNumber)));

            _container.RegisterAsSingle(c => new GameplayStateMachineDisposer());
            _container.RegisterAsSingle(c => new GameplayStatesFactory(c));
            _container.RegisterAsSingle(c => new GameplayFinishConditionService());

            _container.RegisterAsSingle(c =>
            {
                GameplayUIRoot gameplayUIRootPrefab = c.Resolve<ResourcesAssetLoader>().LoadResource<GameplayUIRoot>("Gameplay/UI/GameplayUIRoot");
                return Instantiate(gameplayUIRootPrefab);
            }).NonLazy();

            _container.Initialize();
        }

        private GameplayStateMachine CreateGameplayStateMachine(GameplayInputArgs gameplayInputArgs)
        {
            GameplayStateMachineDisposer disposer = _container.Resolve<GameplayStateMachineDisposer>(); 
            GameplayFinishConditionService gameplayFinishConditionService = _container.Resolve<GameplayFinishConditionService>();

            GameplayStatesFactory gameplayStatesFactory = _container.Resolve<GameplayStatesFactory>();

            InitMainCharacterState initMainCharacterState = gameplayStatesFactory.CreateInitMainCharacterState();
            GameplayStateMachine gameLoopState = gameplayStatesFactory.CreateGameLoopState(gameplayInputArgs);
            DefeatState defeatState = gameplayStatesFactory.CreateDefeatState();
            WinState winState = gameplayStatesFactory.CreateWinState(gameplayInputArgs);

            ActionCondition initMainCharacterToGameLoopStateCondition = new ActionCondition(initMainCharacterState.MainCharacterSetupComplete);

            List<IDisposable> disposables = new List<IDisposable>();
            disposables.Add(initMainCharacterToGameLoopStateCondition);
            disposables.Add(gameLoopState);

            GameplayStateMachine gameplayStateMachine = new GameplayStateMachine(disposables);

            gameplayStateMachine.AddState(initMainCharacterState);
            gameplayStateMachine.AddState(gameLoopState);
            gameplayStateMachine.AddState(defeatState);
            gameplayStateMachine.AddState(winState);

            gameplayStateMachine.AddTransition(
                initMainCharacterState, gameLoopState,
                initMainCharacterToGameLoopStateCondition);

            gameplayStateMachine.AddTransition(
                gameLoopState, winState,
                gameplayFinishConditionService.WinCondition);

            gameplayStateMachine.AddTransition(
                gameLoopState, defeatState,
                gameplayFinishConditionService.DefeatCondition);

            disposer.Set(gameplayStateMachine);

            return gameplayStateMachine;
        }

        private AbilitySelectPopupPresenter _popup;

        private void Update()
        {
            _gameplayStateMachine?.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (Entity entity in _container.Resolve<EntitiesBuffer>().Elements)
                {
                    if(entity.TryGetTeam(out var team) && team.Value == TeamTypes.Enemies)
                    {
                        if (entity.TryGetTakeDamageRequest(out var takeDamageRequest))
                            takeDamageRequest.Invoke(99999);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _container.Resolve<MainHeroHolderService>().MainHero.GetExperience().Value += 150;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _container.Resolve<IPauseService>().Pause();
                _popup = _container.Resolve<AbilityPresentersFactory>().CreateAbilitySelectPopupPresenter(_container.Resolve<MainHeroHolderService>().MainHero);
                _popup.Enable();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                _popup.Disable(() => _container.Resolve<IPauseService>().Unpause());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _container.Resolve<MainHeroHolderService>().MainHero.GetInstantAttackEvent().Invoke();
            }
        }
    }
}