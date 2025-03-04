using Assets.CourseGame.Develop.CommonServices.CoroutinePerfomer;
using Assets.CourseGame.Develop.DI;
using Assets.CourseGame.Develop.Utils.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CourseGame.Develop.CommonServices.Timer
{
    public class TimerService
    {
        private ReactiveVariable<float> _cooldown;

        private ReactiveEvent _cooldownEnded;

        private float _currentTime;

        private ICoroutinePerformer _coroutinePerformer;
        private Coroutine _cooldownProcess;

        public TimerService(
            float cooldown,
            ICoroutinePerformer coroutinePerformer)
        {
            _cooldown = new ReactiveVariable<float>(cooldown);

            _cooldownEnded = new ReactiveEvent();
            _coroutinePerformer = coroutinePerformer;
        }

        public IReadOnlyEvent CooldownEnded => _cooldownEnded;
        public float CurrentTime => _currentTime;
        public bool IsOver => _currentTime <= 0;

        public void Stop()
        {
            if(_cooldownProcess != null)
                _coroutinePerformer.StopPerform(_cooldownProcess);
        }

        public void Restart()
        {
            Stop();

            _cooldownProcess = _coroutinePerformer.StartPerform(CooldownProcess());
        }

        private IEnumerator CooldownProcess()
        {
            _currentTime = _cooldown.Value;

            while(IsOver == false)
            {
                _currentTime -= Time.deltaTime;
                yield return null;
            }

            _cooldownEnded.Invoke();
        }
    }

}
