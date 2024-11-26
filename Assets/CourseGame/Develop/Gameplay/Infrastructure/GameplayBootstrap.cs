using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.DI;
using System.Collections;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Infrastructure
{
    public class GameplayBootstrap : MonoBehaviour
    {
        private DIContainer _container;

        public IEnumerator Run(DIContainer container, GameplayInputArgs gameplayInputArgs)
        {
            _container = container;

            ProcessRegistrations();

            Debug.Log($"���������� ������� ��� ������ {gameplayInputArgs.LevelNumber}");
            Debug.Log("������� ���������");
            Debug.Log("����� ������ ����� �������� ����");

            yield return new WaitForSeconds(1f);
        }

        private void ProcessRegistrations()
        {
            //������ ����������� ��� ����� ��������

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