using Assets.CourseGame.Develop.CommonServices.LoadingScreen;
using Assets.CourseGame.Develop.DI;
using System.Collections;
using UnityEngine;

namespace Assets.CourseGame.Develop.EntryPoint
{
    //Если entry point - это просто глобальные регистрации для старта проекта,
    //то bootstrap - уже инициализация начала работ
    public class Bootstrap : MonoBehaviour
    {
        public IEnumerator Run(DIContainer container)
        {
            ILoadingCurtain loadingCurtain = container.Resolve<ILoadingCurtain>();

            loadingCurtain.Show();

            Debug.Log("Начинается инициализация сервисов");

            //Инициализаций всех (подгрузка данных/конфигов/инит сервисов рекламы/аналитики и тп)

            yield return new WaitForSeconds(1.5f);//инициализация какого-то процесса инициализация

            Debug.Log("Завершается инициализация сервисов проекта, начинается переход на какую-то сцену");

            loadingCurtain.Hide();

            //переход на следующий сцену с помощью сервиса смены сцен
        }
    }
}
