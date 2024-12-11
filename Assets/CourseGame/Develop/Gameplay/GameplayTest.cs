using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Gameplay.Entities;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay
{
    public class GameplayTest : MonoBehaviour
    {
        private DIContainer _container;

        private Entity _ghost;

        public void StartProcess(DIContainer container)
        {
            _container = container;

            _ghost = _container.Resolve<EntityFactory>().CreateGhost(Vector3.zero);

            Debug.Log($"Скорость созданного призрака: {_ghost.GetMoveSpeed().Value}");
        }

        private void Update()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if(_ghost != null)
            {
                _ghost.GetMoveDirection().Value = input;
                _ghost.GetRotationDirection().Value = input;
            }
        }
    }
}
