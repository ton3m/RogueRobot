﻿using Assets.CourseGame.Develop.CommonServices.DataManagment.DataProviders;
using Assets.CourseGame.Develop.Utils.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.CourseGame.Develop.CommonServices.Wallet
{
    public class WalletService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private Dictionary<CurrencyTypes, ReactiveVariable<int>> _currencies = new();

        public WalletService(PlayerDataProvider playerDataProvider)
        {
            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public List<CurrencyTypes> AvailableCurrencies => _currencies.Keys.ToList();

        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type)
            => _currencies[type];

        public bool HasEnough(CurrencyTypes type, int amount)
            => _currencies[type].Value >= amount;

        public void Spend(CurrencyTypes type, int amount)
        {
            if (HasEnough(type, amount) == false)
                throw new ArgumentException(type.ToString());

            _currencies[type].Value -= amount;
        }

        public void Add(CurrencyTypes type, int amount) => _currencies[type].Value += amount;

        public void ReadFrom(PlayerData data)
        {
            foreach (KeyValuePair<CurrencyTypes, int> currency in data.WalletData)
            {
                if (_currencies.ContainsKey(currency.Key))
                    _currencies[currency.Key].Value = currency.Value;
                else
                    _currencies.Add(currency.Key, new ReactiveVariable<int>(currency.Value));
            }
        }

        public void WriteTo(PlayerData data)
        {
            foreach (KeyValuePair<CurrencyTypes, ReactiveVariable<int>> currency in _currencies)
            {
                if (data.WalletData.ContainsKey(currency.Key))
                    data.WalletData[currency.Key] = currency.Value.Value;
                else
                    data.WalletData.Add(currency.Key, currency.Value.Value);
            }
        }
    }
}
