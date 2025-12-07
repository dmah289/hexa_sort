using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        [Header("----- References -----")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource sfxSource;
        
        // Fields
        private Dictionary<string,AudioClip> cachedSfxClips;
            
        protected override void Awake()
        {
            base.Awake();

            cachedSfxClips = new Dictionary<string, AudioClip>();
            EventBus<OnSettingButtonClicked>.Register(onEventWithArgs: OnSettingButtonClicked);

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

        public async UniTask PlaySfx(string key)
        {
            AudioClip clip;
            
            if (cachedSfxClips.TryGetValue(key, out clip))
            {
                sfxSource.PlayOneShot(clip);
                return;
            }

            clip = await Addressables.LoadAssetAsync<AudioClip>(key).ToUniTask();
            if (clip != null)
            {
                if (!cachedSfxClips.ContainsKey(key))
                {
                    cachedSfxClips[key] = clip;
                }
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}