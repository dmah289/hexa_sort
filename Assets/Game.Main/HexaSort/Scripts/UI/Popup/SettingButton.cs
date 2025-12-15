using Cysharp.Threading.Tasks;
using Framework;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Audio;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.MainMenu.SharedUI
{
    public class SettingButton : MButton
    {
        [SerializeField] private GameObject onImg;
        [SerializeField] private eSettingType settingType;

        protected override void Awake()
        {
            base.Awake();
            
            onImg.SetActive(LocalDataManager.GetSettingState(settingType));
        }

        protected override void OnButtonClicked()
        {
            bool curState = LocalDataManager.GetSettingState(settingType);
            bool newState = !curState;
            LocalDataManager.SetSettingState(settingType, newState);
            onImg.SetActive(newState);
            EventBus<OnSettingButtonClicked>.Raise(new OnSettingButtonClicked(settingType, newState));
            
            base.OnButtonClicked();
        }
    }
}