using Assets.CourseGame.Develop.CommonServices.AssetsManagment;
using Assets.CourseGame.Develop.Configs.Common.Wallet;
using Assets.CourseGame.Develop.Configs.Gameplay;
using Assets.CourseGame.Develop.Configs.Gameplay.Abilities;
using Assets.CourseGame.Develop.Configs.Gameplay.Creatures;
using System;

namespace Assets.CourseGame.Develop.CommonServices.ConfigsManagment
{
    public class ConfigsProviderService
    {
        private ResourcesAssetLoader _resourcesAssetLoader;

        public ConfigsProviderService(ResourcesAssetLoader resourcesAssetLoader)
        {
            _resourcesAssetLoader = resourcesAssetLoader;
        }

        public StartWalletConfig StartWalletConfig { get; private set; }

        public AbilitiesConfigsContainer AbilitiesConfigsContaier { get; private set; }

        public CurrencyIconsConfig CurrencyIconsConfig { get; private set; }

        public LevelListConfig LevelsListConfig { get; private set; }  
        
        public MainHeroConfig MainHeroConfig { get; private set; }

        public ExperienceForUpgradeLevelConfig ExperienceForUpgradeLevelConfig { get; private set; }

        public void LoadAll()
        {
            //подгружать конфиги из ресурсов
            LoadStartWalletConfig();
            LoadCurrencyIconsConfig();
            LoadLevelsListConfig();
            LoadMainHeroConfig();
            LoadAbilitiesConfigsContaier();
            LoadExperienceForUpgradeLevelConfig();
        }

        private void LoadExperienceForUpgradeLevelConfig()
            => ExperienceForUpgradeLevelConfig = _resourcesAssetLoader.LoadResource<ExperienceForUpgradeLevelConfig>("Configs/Gameplay/ExperienceForUpgradeLevelConfig");

        private void LoadAbilitiesConfigsContaier()
            => AbilitiesConfigsContaier = _resourcesAssetLoader.LoadResource<AbilitiesConfigsContainer>("Configs/Gameplay/Abilities/AbilitiesConfigsContainer");

        private void LoadMainHeroConfig()
            => MainHeroConfig = _resourcesAssetLoader.LoadResource<MainHeroConfig>("Configs/Gameplay/Creatures/MainHeroConfig");

        private void LoadStartWalletConfig()
            => StartWalletConfig = _resourcesAssetLoader.LoadResource<StartWalletConfig>("Configs/Common/Wallet/StartWalletConfig");

        private void LoadCurrencyIconsConfig()
            => CurrencyIconsConfig = _resourcesAssetLoader.LoadResource<CurrencyIconsConfig>("Configs/Common/Wallet/CurrencyIconsConfig");

        private void LoadLevelsListConfig()
            => LevelsListConfig = _resourcesAssetLoader.LoadResource<LevelListConfig>("Configs/Gameplay/Levels/LevelListConfig");
    }
}
