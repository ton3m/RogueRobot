using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.CourseGame.Develop.CommonServices.SceneManagment
{
    public class DefaultSceneLoader : ISceneLoader
    {
        public IEnumerator LoadAsync(SceneID sceneID, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            AsyncOperation waitLoding = SceneManager.LoadSceneAsync(sceneID.ToString(), loadSceneMode);

            while(waitLoding.isDone == false)
                yield return null;
        }
    }
}
