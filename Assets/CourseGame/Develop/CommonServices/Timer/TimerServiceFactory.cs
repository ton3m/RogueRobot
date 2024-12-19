using Assets.CourseGame.Develop.CommonServices.CoroutinePerfomer;
using Assets.CourseGame.Develop.DI;

namespace Assets.CourseGame.Develop.CommonServices.Timer
{
    public class TimerServiceFactory
    {
        private DIContainer _container;

        public TimerServiceFactory(DIContainer container)
        {
            _container = container;
        }

        public TimerService Create(float cooldown)
            => new TimerService(cooldown, _container.Resolve<ICoroutinePerformer>());
    }
}
