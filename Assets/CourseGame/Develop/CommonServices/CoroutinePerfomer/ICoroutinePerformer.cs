using System.Collections;
using UnityEngine;

namespace Assets.CourseGame.Develop.CommonServices.CoroutinePerfomer
{
    public interface ICoroutinePerformer
    {
        Coroutine StartPerform(IEnumerator coroutineFunction);
        void StopPerform(Coroutine coroutine);
    }
}
