using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Gameplay.Features.StatsFeature
{
    public interface IStatsEffect
    {
        void ApplyTo(Dictionary<StatTypes, float> stats);
    }
}
