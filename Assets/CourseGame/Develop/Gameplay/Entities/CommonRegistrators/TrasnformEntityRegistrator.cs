using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.Entities.CommonRegistrators
{
    public class TrasnformEntityRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private Transform _transform;

        public override void Register(Entity entity)
        {
            entity.AddValue(EntityValues.Transform, _transform);
        }
    }
}
