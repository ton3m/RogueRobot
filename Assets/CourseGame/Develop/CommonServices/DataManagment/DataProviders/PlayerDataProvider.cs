using Assets.CourseGame.Develop.CommonServices.ConfigsManagment;
using Assets.CourseGame.Develop.CommonServices.Wallet;
using Assets.CourseGame.Develop.Gameplay.Features.StatsFeature;
using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders
{
    public class PlayerDataProvider : DataProvider<PlayerData>
    {
        private ConfigsProviderService _configsProviderService;

        //тут будем передавать сервис конфигов
        public PlayerDataProvider(
            ISaveLoadSerivce saveLoadService,
            ConfigsProviderService configsProviderService) : base(saveLoadService)
        {
            _configsProviderService = configsProviderService;
        }

        protected override PlayerData GetOriginData()
        {
            return new PlayerData()
            {
                WalletData = InitWalletData(),
                CompletedLevels = new(),
                StatsUpgradeLevel = InintStatsUpgradesLevels()
            };
        }

        private Dictionary<StatTypes, int> InintStatsUpgradesLevels()
        {
            Dictionary<StatTypes, int> statUpgradesLevels = new();

            foreach (StatTypes statType in Enum.GetValues(typeof(StatTypes)))
                statUpgradesLevels.Add(statType, 1);

            return statUpgradesLevels;
        }

        private Dictionary<CurrencyTypes, int> InitWalletData()
        {
            Dictionary<CurrencyTypes, int> walletData = new();

            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
                walletData.Add(currencyType, _configsProviderService.StartWalletConfig.GetStartValueFor(currencyType));

            return walletData;
        }
    }
}
