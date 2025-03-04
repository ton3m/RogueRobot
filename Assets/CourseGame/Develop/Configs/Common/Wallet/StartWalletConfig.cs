using Assets.CourseGame.Develop.CommonServices.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CourseGame.Develop.Configs.Common.Wallet
{
    [CreateAssetMenu(menuName = "Configs/Common/Wallet/NewStartWalletConfig", fileName = "StartWalletConfig")]
    public class StartWalletConfig : ScriptableObject
    {
        [SerializeField] private List<CurrencyConfig> _values;

        private void OnValidate()
        {
            //можно проверить точно ли все элементы енама представлены в конфиге
            //нет ли дупликатов и тд
        }

        public int GetStartValueFor(CurrencyTypes currencyType) => _values.First(config => config.Type == currencyType).Value;

        [Serializable]
        private class CurrencyConfig
        {
            [field: SerializeField] public CurrencyTypes Type { get; private set; } 
            [field: SerializeField] public int Value { get; private set; }
        }
    }
}
