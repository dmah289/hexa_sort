using System;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace HexaSort.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private AudioMixer audioMixer;

        public GameObject gameplayAudio;
        [SerializeField] private AudioSource bgSource;

        protected override void Awake()
        {
            base.Awake();
        }

        // private void Start()
        // {
        //     for (int i = 0; i < Enum.GetValues(typeof(SettingType)).Length; i++)
        //         ToogleSetting((SettingType)i, GameData.SettingState[i] == 1);
        // }
        //
        // public void ToogleSetting(SettingType type, bool active)
        // {
        //     if (type == SettingType.Vibration)
        //         return;
        //     
        //     audioMixer.SetFloat(type.ToString(), active ? 0f : -80f);
        // }
        //
        // public void Vibrate()
        // {
        //     if (Application.isMobilePlatform && GameData.SettingState[(byte)SettingType.Vibration] == 1)
        //     {
        //         Handheld.Vibrate();
        //     }
        // }
    }
}