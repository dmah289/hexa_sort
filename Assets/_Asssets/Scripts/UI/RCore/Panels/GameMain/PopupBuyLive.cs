using LiveOps;
using LiveOps.Config;
using LiveOps.UI;
using RCore;
using Runtime.Definition;
using Runtime.Manager.Toast;
using Dessentials.Features.ABTesting;
using LiveOps.Data;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class PopupBuyLive : BuyWithCoinPanel
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _liveAmountText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TimeRemainElement _timeRemainElement;

        protected override int BuyPriceConfig
            => ABTestManager.Instance.CoinInOutConfig.Value.refillLivePrice;

        protected override void BeforeShowing()
        {
            base.BeforeShowing();

            SyncLiveAmount();

            EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
        }

        protected override void AfterHiding()
        {
            base.AfterHiding();

            if (_timeRemainElement.HasInitialized)
            {
                _timeRemainElement.Dispose();
            }

            EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
        }

        private void OnCurrencyChanged(CurrencyChangedEvent e)
        {
            if (e.currency == LiveOps.IDs.Currency.c_Live)
            {
                SyncLiveAmount();
            }
        }

        private void SyncLiveAmount()
        {
            if (_timeRemainElement.HasInitialized)
            {
                _timeRemainElement.Dispose();
            }

            var lives = DataManager.Instance.PlayerData.live;

            if (lives >= LiveOps.Constants.MAX_LIVE)
            {
                _timeText.text = "FULL";
            }
            else
            {
                _timeRemainElement.Initialize();

                var regenTime = DataManager.Instance.PlayerData.liveRegenTime;

                _timeRemainElement.SetTimeRemain(regenTime, Utilities.TimeFormatType.Mmss);
            }

            _liveAmountText.text = lives.ToString();
        }

        protected override void OnBuyWithCoinButtonClicked()
        {
            base.OnBuyWithCoinButtonClicked();

            if (DataManager.Instance.HasFullLives)
            {
                ToastManager.Instance.Show(LocalizationCommon.Get(LocalizationCommon.Live_fulled).ToString());
            }
            else
            {
                if (DataManager.Instance.IsEnoughCoins(_buyPrice))
                {
                    DataManager.Instance.DeductCoins(_buyPrice, Constant.GROUP_PLACEMENT_HOME, Constant.PLACEMENT_BUY_LIVE);

                    RefillLives();
                }
            }

        }

        protected override void OnWatchAdButtonClicked()
        {
            base.OnWatchAdButtonClicked();

            if (DataManager.Instance.HasFullLives)
            {
                ToastManager.Instance.Show(LocalizationCommon.Get(LocalizationCommon.Live_fulled).ToString());
            }
            else
            {
                AdsManager.instance.ShowRewardVideo(RefillLives, "popup_buy_live");
            }
        }

        protected virtual void RefillLives()
        {
            ToastManager.Instance.Show(LocalizationCommon.Get(LocalizationCommon.Lives_refilled).ToString());

            var lives = DataManager.Instance.PlayerData.live;

            DataManager.Instance.AddLives(LiveOps.Constants.MAX_LIVE - lives, Constant.GROUP_PLACEMENT_HOME, Constant.PLACEMENT_BUY_LIVE);

            HomeUIRoot.Instance.tweenFXManager.TweenCurrency(IDs.Currency.c_Live, null);

            Back();
        }
    }
}