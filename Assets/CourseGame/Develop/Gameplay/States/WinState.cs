﻿using Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders;
using Assets.CourseGame.Develop.CommonServices.LevelsManagment;
using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.CommonServices.Wallet;
using Assets.CourseGame.Develop.Gameplay.Features.InputFeature;
using Assets.CourseGame.Develop.Gameplay.Features.MainHeroFeature;
using Assets.CourseGame.Develop.Gameplay.Features.PauseFeature;
using Assets.CourseGame.Develop.Utils.StateMachineBase;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.States
{
    public class WinState : EndGameState, IUpdatableState
    {
        private CompletedLevelsService _completedLevelsService;
        private PlayerDataProvider _playerDataProvider;
        private GameplayInputArgs _gameplayInputArgs;
        private SceneSwitcher _sceneSwitcher;
        private readonly WalletService _walletService;
        private readonly MainHeroHolderService _mainHeroHolderService;

        public WinState(
            CompletedLevelsService completedLevelsService,
            PlayerDataProvider playerDataProvider,
            GameplayInputArgs gameplayInputArgs,
            SceneSwitcher sceneSwitcher,
            IPauseService pauseService, 
            IInputService inputService,
            WalletService walletService,
            MainHeroHolderService mainHeroHolderService) : base(pauseService, inputService)
        {
            _completedLevelsService = completedLevelsService;
            _playerDataProvider = playerDataProvider;
            _gameplayInputArgs = gameplayInputArgs;
            _sceneSwitcher = sceneSwitcher;
            _walletService = walletService;
            _mainHeroHolderService = mainHeroHolderService;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("ПОБЕДА!");

            _walletService.Add(CurrencyTypes.Gold, _mainHeroHolderService.MainHero.GetCoins().Value);

            _completedLevelsService.TryAddLevelToCompleted(_gameplayInputArgs.LevelNumber);
            _playerDataProvider.Save();
        }

        public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _sceneSwitcher.ProcessSwitchSceneFor(new OutputGameplayArgs(new MainMenuInputArgs()));
            }
        }
    }
}
