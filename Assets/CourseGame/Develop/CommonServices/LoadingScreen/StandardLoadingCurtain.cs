using UnityEngine;

namespace Assets.CourseGame.Develop.CommonServices.LoadingScreen
{
    public class StandardLoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        public bool IsShown => gameObject.activeSelf;

        private void Awake()
        {
            Hide();
            DontDestroyOnLoad(this);
        }

        public void Hide() => gameObject.SetActive(false);

        public void Show() => gameObject.SetActive(true);
    }
}
