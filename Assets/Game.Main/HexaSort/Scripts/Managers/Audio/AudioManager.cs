using System;
using Game.Main.HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace HexaSort.Audio
{
    public enum eSettingType : byte
    {
        Music,
        SFX,
        Vibration
    }
    
    public struct OnSettingButtonClicked : IEventDTO
    {
        public eSettingType settingType;
        public bool isActive;

        public OnSettingButtonClicked(eSettingType type, bool active)
        {
            settingType = type;
            isActive = active;
        }
    }
    
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private AudioMixer audioMixer;

        public GameObject gameplayAudio;
        [SerializeField] private AudioSource sfxSource;

        protected override void Awake()
        {
            base.Awake();
            
            EventBus<OnSettingButtonClicked>.Register(onEventWithArgs: OnSettingButtonClicked);
        }

        private void Start()
        {
            for (int i = 0; i < Enum.GetValues(typeof(eSettingType)).Length; i++)
            {
                eSettingType type = (eSettingType)i;
                bool isActive = LocalDataManager.GetSettingState(type);
                
                if (type == eSettingType.Vibration)
                    return;
            
                audioMixer.SetFloat(type.ToString(), isActive ? 0f : -80f);
            }
        }
        
        public void OnSettingButtonClicked(OnSettingButtonClicked data)
        {
            LocalDataManager.SetSettingState(data.settingType, data.isActive);
            
            if (data.settingType == eSettingType.Vibration)
                return;
            
            audioMixer.SetFloat(data.settingType.ToString(), data.isActive ? 0f : -80f);
        }
        
        public void Vibrate()
        {
            if (Application.isMobilePlatform 
                && LocalDataManager.GetSettingState(eSettingType.Vibration))
            {
                Handheld.Vibrate();
            }
        }
    }
}