using Cysharp.Threading.Tasks;
using Framework;
using Framework.UI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.MainMenu.SharedUI
{
    public class SettingButton : ScaleAnimButton
    {
        [SerializeField] private GameObject onImg;
        [SerializeField] private int selfIndex;

        protected override void Awake()
        {
            base.Awake();
            
            int curState = PlayerPrefs.GetInt(ConstantKey.SettingsKeys[selfIndex], 1);
            onImg.SetActive(curState == 1);
        }

        protected override void OnButtonClicked()
        {
            int curState = PlayerPrefs.GetInt(ConstantKey.SettingsKeys[selfIndex], 1);
            int newState = 1 - curState;
            PlayerPrefs.SetInt(ConstantKey.SettingsKeys[selfIndex], newState);
            print(PlayerPrefs.GetInt(ConstantKey.SettingsKeys[selfIndex]));
            onImg.SetActive(newState == 1);
            
            base.OnButtonClicked();
        }
    }
}