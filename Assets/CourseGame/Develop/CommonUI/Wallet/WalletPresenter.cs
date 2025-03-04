using Assets.CourseGame.Develop.CommonServices.Wallet;
using Assets.CourseGame.Develop.Configs.Common.Wallet;
using Assets.CourseGame.Develop.DI;
using System;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.CommonUI.Wallet
{
    public class WalletPresenter : IInitializable, IDisposable
    {
        //модель
        private WalletService _walletService;
        private WalletPresenterFactory _factory;
        private List<CurrencyPresenter> _currencyPresenters = new();

        //вью
        private IconsWithTextListView _view;

        public WalletPresenter(WalletService walletService, IconsWithTextListView view, WalletPresenterFactory factory)
        {
            _walletService = walletService;
            _view = view;
            _factory = factory;
        }

        public void Initialize()
        {
            foreach (CurrencyTypes currencyType in _walletService.AvailableCurrencies)
            {
                IconWithText currencyView = _view.SpawnElement();

                CurrencyPresenter currencyPresenter = _factory.CreateCurrencyPresenter(currencyView, currencyType);

                currencyPresenter.Initialize();
                _currencyPresenters.Add(currencyPresenter); 
            }
        }

        public void Dispose()
        {
            foreach (CurrencyPresenter currencyPresenter in _currencyPresenters)
            {
                _view.Remove(currencyPresenter.View);
                currencyPresenter.Dispose();
            }

            _currencyPresenters.Clear();
        }
    }
}
