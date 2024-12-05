﻿using Assets.CourseGame.Develop.CommonServices.LevelsManagment;
using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using UnityEngine;

namespace Assets.CourseGame.Develop.MainMenu.LevelsMenuFeature.LevelsMenuPopup
{
    public class LevelTilePresenter
    {
        private const int FirstLevel = 1;

        private readonly CompletedLevelsService _levelsService;
        private readonly SceneSwitcher _sceneSwitcher;
        private readonly int _levelNumber;

        private LevelTileView _view;

        private bool _isBlocked;

        public LevelTilePresenter(
            CompletedLevelsService levelsService, 
            SceneSwitcher sceneSwitcher, 
            int levelNumber, 
            LevelTileView view)
        {
            _levelsService = levelsService;
            _sceneSwitcher = sceneSwitcher;
            _levelNumber = levelNumber;
            _view = view;
        }

        public LevelTileView View => _view;

        public void Enable()
        {
            _isBlocked = _levelNumber != FirstLevel && PreviousLevelCompleted() == false;

            _view.SetLevel(_levelNumber.ToString());

            if (_isBlocked)
            {
                _view.SetBlock();
            }
            else
            {
                if (_levelsService.IsLevelCompleted(_levelNumber))
                    _view.SetComplete();
                else
                    _view.SetActive();
            }

            _view.Clicked += OnViewClicked;
        }

        public void Disable()
        {
            _view.Clicked -= OnViewClicked;
        }

        private void OnViewClicked()
        {
            if (_isBlocked)
            {
                Debug.Log("Уровень заблокирован, пройдите предыдущий");
                return;
            }

            if (_levelsService.IsLevelCompleted(_levelNumber))
            {
                Debug.Log("Уровень уже завершен");
                return;
            }

            _sceneSwitcher.ProcessSwitchSceneFor(new OutputMainMenuArgs(new GameplayInputArgs(_levelNumber)));
        }

        private bool PreviousLevelCompleted() => _levelsService.IsLevelCompleted(_levelNumber - 1);
    }
}
