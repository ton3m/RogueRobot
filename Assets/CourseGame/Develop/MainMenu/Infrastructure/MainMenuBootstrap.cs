using Assets.CourseGame.Develop.CommonServices.DataManagment;
using Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders;
using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using Assets.CourseGame.Develop.DI;
using System.Collections;
using UnityEngine;

public class MainMenuBootstrap : MonoBehaviour
{
    private DIContainer _container;

    public IEnumerator Run(DIContainer container, MainMenuInputArgs mainMenuInputArgs)
    {
        _container = container;

        ProcessRegistrations();

        yield return new WaitForSeconds(1f);
    }

    private void ProcessRegistrations()
    {
        //Делаем регистрации для сцены геймплея
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _container.Resolve<SceneSwitcher>().ProcessSwitchSceneFor(new OutputMainMenuArgs(new GameplayInputArgs(2)));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ISaveLoadSerivce saveLoadSerivce = _container.Resolve<ISaveLoadSerivce>();

            if(saveLoadSerivce.TryLoad(out PlayerData playerData))
            {
                playerData.Money++;
                playerData.CompletedLevels.Add(playerData.Money);

                saveLoadSerivce.Save(playerData);
            }
            else
            {
                PlayerData originPlayerData = new PlayerData()
                {
                    Money = 0,
                    CompletedLevels = new()
                };

                saveLoadSerivce.Save(originPlayerData);
            }
        }
    }
}
