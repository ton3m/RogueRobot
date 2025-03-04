﻿using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.Gameplay.Features.InputFeature;
using Assets.CourseGame.Develop.Gameplay.Features.PauseFeature;
using Assets.CourseGame.Develop.Utils.StateMachineBase;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.States
{
    public class DefeatState : EndGameState, IUpdatableState
    {
        private SceneSwitcher _sceneSwitcher;

        public DefeatState(
            SceneSwitcher sceneSwitcher,
            IPauseService pauseService, 
            IInputService inputService) : base(pauseService, inputService)
        {
            _sceneSwitcher = sceneSwitcher;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("ПОРАЖЕНИЕ");
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
