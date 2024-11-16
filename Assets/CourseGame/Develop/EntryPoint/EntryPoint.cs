using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.CommonServices.CoroutinePerfomer;
using Assets.CourseGame.Develop.CommonServices.LoadingScreen;
using Assets.CourseGame.Develop.DI;
using UnityEngine;

namespace Assets.CourseGame.Develop.EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Bootstrap _gameBootstrap;

        private void Awake()
        {
            SetupAppSettings();

            DIContainer projectContainer = new DIContainer();

            //регистрация сервисов на целый проект
            //Аналог global context из популярных DI фреймворков
            //Самый родительский контейнер для всех будущих

            RegisterResourcesAssetLoader(projectContainer);
            RegisterCoroutinePerformer(projectContainer);
            RegisterSceneLoader(projectContainer);

            //все регистрации прошли
            projectContainer.Resolve<ICoroutinePerformer>().StartPerform(_gameBootstrap.Run(projectContainer));
        }

        private void SetupAppSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 144;
        }

        private void RegisterResourcesAssetLoader(DIContainer container) 
            => container.RegisterAsSingle(c => new ResourcesAssetLoader());

        private void RegisterCoroutinePerformer(DIContainer container)
        {
            container.RegisterAsSingle<ICoroutinePerformer>(c =>
            {
                ResourcesAssetLoader resourcesAssetLoader = c.Resolve<ResourcesAssetLoader>();

                CoroutinePerformer coroutinePerformerPrefab = resourcesAssetLoader
                .LoadResource<CoroutinePerformer>(InfrastructureAssetPaths.CoroutinePerformerPath);

                return Instantiate(coroutinePerformerPrefab);
            });
        }

        private void RegisterSceneLoader(DIContainer container)
        {
            container.RegisterAsSingle<ILoadingCurtain>(c =>
            {
                ResourcesAssetLoader resourcesAssetLoader = c.Resolve<ResourcesAssetLoader>();

                StandardLoadingCurtain standardLoadingCurtainPrefab = resourcesAssetLoader
                .LoadResource<StandardLoadingCurtain>(InfrastructureAssetPaths.LoadingCurtainPath);

                return Instantiate(standardLoadingCurtainPrefab);
            });
        }

    }
}
