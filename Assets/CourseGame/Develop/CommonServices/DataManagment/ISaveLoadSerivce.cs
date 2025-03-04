namespace Assets.CourseGame.Develop.CommonServices.DataManagment
{
    public interface ISaveLoadSerivce
    {
        bool TryLoad<TData>(out TData data) where TData : ISaveData;
        void Save<TData>(TData data) where TData : ISaveData;   
    }
}
