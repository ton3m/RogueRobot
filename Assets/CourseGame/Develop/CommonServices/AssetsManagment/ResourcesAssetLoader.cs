using UnityEngine;

namespace Assets.CourseGame.Develop.CommonServices.AssetsManagment
{
    public class ResourcesAssetLoader
    {
        public T LoadResource<T>(string resourcePath) where T : Object
            => Resources.Load<T>(resourcePath);
    }
}