using Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.CommonServices.LevelsManagment
{
    public class CompletedLevelsService : IDataReader<PlayerData>, IDataWriter<PlayerData>  
    {
        private List<int> _completedLevels = new();

        public CompletedLevelsService(PlayerDataProvider playerDataProvider)
        {
            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public bool IsLevelCompleted(int levelNumber) => _completedLevels.Contains(levelNumber);

        public bool TryAddLevelToCompleted(int levelNumber)
        {
            if (IsLevelCompleted(levelNumber))
                return false;

            _completedLevels.Add(levelNumber);
            return true;    
        }

        public void ReadFrom(PlayerData data)
        {
            _completedLevels.Clear();   
            _completedLevels.AddRange(data.CompletedLevels);
        }

        public void WriteTo(PlayerData data)
        {
            data.CompletedLevels.Clear();
            data.CompletedLevels.AddRange(_completedLevels);
        }
    }
}
