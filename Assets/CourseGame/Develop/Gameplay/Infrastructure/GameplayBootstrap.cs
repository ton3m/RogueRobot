using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Entities;
using System.Collections;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Infrastructure
{
    public class GameplayBootstrap : MonoBehaviour
    {
        private DIContainer _container;

        [SerializeField] private GameplayTest _gameplayTest; //на время тестов, потом удалим

        public IEnumerator Run(DIContainer container, GameplayInputArgs gameplayInputArgs)
        {
            _container = container;

            ProcessRegistrations();

            Debug.Log($"Подгружаем ресурсы для уровня {gameplayInputArgs.LevelNumber}");
            Debug.Log("Создаем персонажа");
            Debug.Log("Сцена готова можно начинать игру");

            _gameplayTest.StartProcess(_container);

            yield return new WaitForSeconds(1f);
        }

        private void ProcessRegistrations()
        {
            //Делаем регистрации для сцены геймплея
            _container.RegisterAsSingle(c => new EntityFactory(c));

            _container.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _container.Resolve<SceneSwitcher>().ProcessSwitchSceneFor(new OutputGameplayArgs(new MainMenuInputArgs()));
            }
        }
    }
}