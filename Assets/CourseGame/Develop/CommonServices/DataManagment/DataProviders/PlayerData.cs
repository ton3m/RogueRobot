using Assets.CourseGame.Develop.CommonServices.Wallet;
using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders
{
    [Serializable]
    public class PlayerData : ISaveData
    {
        public Dictionary<CurrencyTypes, int> WalletData;
        public List<int> CompletedLevels;
    }
}
