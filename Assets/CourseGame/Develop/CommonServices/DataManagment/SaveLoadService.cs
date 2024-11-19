namespace Assets.CourseGame.Develop.CommonServices.DataManagment
{
    public class SaveLoadService : ISaveLoadSerivce
    {
        private readonly IDataSerializer _serializer;   
        private readonly IDataRepository _repository;

        public SaveLoadService(IDataSerializer serializer, IDataRepository repository)
        {
            _serializer = serializer;
            _repository = repository;
        }

        public void Save<TData>(TData data) where TData : ISaveData
        {
            string serializeData = _serializer.Serialize(data);
            _repository.Write(SaveDataKeys.GetKeyFor<TData>(), serializeData);
        }

        public bool TryLoad<TData>(out TData data) where TData : ISaveData
        {
            string key = SaveDataKeys.GetKeyFor<TData>();

            if(_repository.Exists(key) == false)
            {
                data = default(TData);
                return false;
            }

            string serializedData = _repository.Read(key);  
            data = _serializer.Deserialize<TData>(serializedData);
            return true;
        }
    }
}
