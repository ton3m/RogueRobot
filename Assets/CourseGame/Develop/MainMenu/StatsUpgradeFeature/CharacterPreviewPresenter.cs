using Assets.CourseGame.Develop.CommonServices.CoroutinePerfomer;
using Assets.CourseGame.Develop.CommonServices.SceneManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.CourseGame.Develop.MainMenu.StatsUpgradeFeature
{
    public class CharacterPreviewPresenter
    {
        private ISceneLoader _sceneLoader;
        private ICoroutinePerformer _coroutinePerformer;

        public CharacterPreviewPresenter(ISceneLoader sceneLoader, ICoroutinePerformer coroutinePerformer)
        {
            _sceneLoader = sceneLoader;
            _coroutinePerformer = coroutinePerformer;
        }

        public void Enable()
        {
            _coroutinePerformer.StartPerform(_sceneLoader.LoadAsync(SceneID.CharacterPreviewScene, LoadSceneMode.Additive));
        }

        public void Disable()
        {
            _coroutinePerformer.StartPerform(_sceneLoader.UnloadAsync(SceneID.CharacterPreviewScene));
        }
    }
}
