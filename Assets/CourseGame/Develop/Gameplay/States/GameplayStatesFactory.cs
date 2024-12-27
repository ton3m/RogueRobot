using Assets.CourseGame.Develop.CommonServices.ConfigsManagment;
using Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders;
using Assets.CourseGame.Develop.CommonServices.LevelsManagment;
using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Features.GameModeStagesFeature;
using Assets.CourseGame.Develop.Gameplay.Features.InputFeature;
using Assets.CourseGame.Develop.Gameplay.Features.MainHeroFeature;
using Assets.CourseGame.Develop.Gameplay.Features.PauseFeature;
using Assets.CourseGame.Develop.Utils.Conditions;
using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Gameplay.States
{
    public class GameplayStatesFactory
    {
        private DIContainer _container;

        public GameplayStatesFactory(DIContainer container)
        {
            _container = container;
        }

        public InitMainCharacterState CreateInitMainCharacterState()
        {
            return new InitMainCharacterState(_container.Resolve<MainHeroFactory>(), _container.Resolve<ConfigsProviderService>());
        }

        public NextStagePreperationState CreateNextStagePreperationState()
        {
            return new NextStagePreperationState(_container.Resolve<EntityFactory>());
        }

        public StageProcessState CreateStageProcessState(GameplayInputArgs gameplayInputArgs)
        {
            return new StageProcessState(
                _container.Resolve<ConfigsProviderService>().LevelsListConfig.GetBy(gameplayInputArgs.LevelNumber),
                _container.Resolve<GameModesFactory>(),
                _container.Resolve<StageProviderService>());
        }

        public WinState CreateWinState(GameplayInputArgs gameplayInputArgs)
        {
            return new WinState(
                _container.Resolve<CompletedLevelsService>(),
                _container.Resolve<PlayerDataProvider>(),
                gameplayInputArgs,
                _container.Resolve<SceneSwitcher>(),
                _container.Resolve<IPauseService>(),
                _container.Resolve<IInputService>());
        }

        public DefeatState CreateDefeatState()
        {
            return new DefeatState(
                _container.Resolve<SceneSwitcher>(),
                _container.Resolve<IPauseService>(),
                _container.Resolve<IInputService>());
        }

        public GameplayStateMachine CreateGameLoopState(GameplayInputArgs gameplayInputArgs)
        {
            GameplayStatesFactory gameplayStatesFactory = _container.Resolve<GameplayStatesFactory>();
            GameplayFinishConditionService gameplayFinishConditionService = _container.Resolve<GameplayFinishConditionService>();
            StageProviderService stageProviderService = _container.Resolve<StageProviderService>();

            NextStagePreperationState nextStagePreperationState = gameplayStatesFactory.CreateNextStagePreperationState();
            StageProcessState stageProcessState = gameplayStatesFactory.CreateStageProcessState(gameplayInputArgs);

            ActionCondition preperationToStageProcesStateCondition = new ActionCondition(nextStagePreperationState.OnNextStageTriggerComplete);

            ActionCondition stageProcessToPreperationStateCondition = new ActionCondition(stageProcessState.StageComplete);

            gameplayFinishConditionService.WinCondition
                .Add(new ActionCondition(nextStagePreperationState.OnNextStageTriggerComplete))
                .Add(new FuncCondition(() => stageProviderService.StageResult == StageResult.Completed
                && stageProviderService.HasNextStage() == false));

            List<IDisposable> disposables = new List<IDisposable>();
            disposables.Add(preperationToStageProcesStateCondition);
            disposables.Add(stageProcessToPreperationStateCondition);

            GameplayStateMachine gameplayLoopState = new GameplayStateMachine(disposables);

            gameplayLoopState.AddState(nextStagePreperationState);
            gameplayLoopState.AddState(stageProcessState);

            gameplayLoopState.AddTransition(
                nextStagePreperationState, stageProcessState,
                preperationToStageProcesStateCondition);

            gameplayLoopState.AddTransition(
                stageProcessState, nextStagePreperationState,
                stageProcessToPreperationStateCondition);

            return gameplayLoopState;
        }
    }
}
