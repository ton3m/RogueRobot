using Assets.CourseGame.Develop.Gameplay.Entities;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Gameplay.AI.Sensors
{
    public interface ITargetSelector
    {
        bool TrySelectTarget(IEnumerable<Entity> targetsm, out Entity finndedTarget);
    }
}
