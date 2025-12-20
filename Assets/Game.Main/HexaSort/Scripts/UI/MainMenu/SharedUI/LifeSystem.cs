using System;
using System.Globalization;
using DG.Tweening;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.MainMenu.SharedUI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.MainMenu.SharedUI
{
    public struct LifeChangedEventDTO : IEventDTO
    {
        public int amount;
        public LifeChangedEventDTO(int amount)
        {
            this.amount = amount;
        }
    }
    
    public class LifeSystem : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Text counter;
        [SerializeField] private Text timer;
        [SerializeField] private ParticleSystem coinHitEffect;

        [Header("Settings")] 
        [SerializeField] private float targetScale;
        [SerializeField] private float animDuration;

        
        [Header("Life System Config")]
        private float countdownRemaining;
        private DateTime lastSaveTime;
        [SerializeField] private bool isTimerRunning;
        [SerializeField] private float timeForOneLife;
        
        public float CurCountdown
        {
            get => countdownRemaining;
            set
            {
                countdownRemaining = Mathf.Max(value, 0);
                UpdateTimerDisplay();
            }
        }
        
        public RectTransform TargetRectTransform => counter.rectTransform;

        #region Unity APIs

        private void Awake()
        {
            EventBus<ResourceChangedEventDTO>.Register(onEventWithArgs: OnLifeCounterChanged);
        }

        private void OnEnable()
        {
            LoadLifeData();
        }
        
        private void Update()
        {
            if (isTimerRunning)
            {
                CurCountdown -= Time.deltaTime;

                if (CurCountdown <= 0)
                {
                    LocalDataManager.CurrentLife++;
                    if(LocalDataManager.CurrentLife < ConstantKey.MAX_LIFE)
                        CurCountdown = timeForOneLife;
                }
            }
        }

        private void OnDisable()
        {
            SaveCounterConfigs();
        }

        private void OnApplicationQuit()
        {
            SaveCounterConfigs();
        }

        #endregion

        #region Class Methods

        public void LoadLifeData()
        {
            if (!PlayerPrefs.HasKey(ConstantKey.LastSaveTimeKey))
            {
                CurCountdown = 0;
            }
            else
            {
                // Restore last countdown
                CurCountdown = LocalDataManager.LastCountdownRemaining;
                
                // Calculate elapsed time
                TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(LocalDataManager.LastLifeSaveTime);
                float totalSecondsElapsed = (float)elapsedTime.TotalSeconds;
                
                if (totalSecondsElapsed < CurCountdown)
                {
                    CurCountdown -= totalSecondsElapsed;
                }
                else
                {
                    totalSecondsElapsed -= LocalDataManager.LastCountdownRemaining;
                    LocalDataManager.CurrentLife++;
                    
                    int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);
                    if (livesToAdd > 0)
                    {
                        LocalDataManager.CurrentLife += livesToAdd;
                        CurCountdown = timeForOneLife - totalSecondsElapsed % timeForOneLife;
                    }
                    else
                    {
                        CurCountdown = timeForOneLife - totalSecondsElapsed;
                    }
                }
                
            }
            
            UpdateLifeCounterDisplay();
        }

        private void UpdateLifeCounterDisplay()
        {
            counter.text = $"{LocalDataManager.CurrentLife}";

            if (LocalDataManager.CurrentLife == ConstantKey.MAX_LIFE)
            {
                isTimerRunning = false;
                timer.text = "Max";
                CurCountdown = timeForOneLife;
            }
            else isTimerRunning = true;
        }

        private void UpdateTimerDisplay()
        {
            if (LocalDataManager.CurrentLife < ConstantKey.MAX_LIFE)
            {
                int minutes = Mathf.FloorToInt(countdownRemaining / 60);
                int seconds = Mathf.FloorToInt(countdownRemaining % 60);
                timer.text = $"{minutes:00}:{seconds:00}";
            }
        }

        public void FlashOnOutOfLife()
        {
            counter.DOKill();
            counter.rectTransform.DOScale(targetScale, animDuration).From(1).SetLoops(4, LoopType.Yoyo)
                .OnComplete(() => counter.rectTransform.localScale = Vector3.one);
            counter.DOFade(1, animDuration).From(0.3f).SetLoops(3, LoopType.Yoyo);
        }
        
        private void SaveCounterConfigs()
        {
            LocalDataManager.LastLifeSaveTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            LocalDataManager.LastCountdownRemaining = countdownRemaining;
        }

        public void PlayCoinHitEffect()
        {
            coinHitEffect.Play();
        }
        
        public void OnLifeCounterChanged(ResourceChangedEventDTO dto)
        {
            if(dto.ResourceType == eResourceType.Life)
                UpdateLifeCounterDisplay();
        }

        #endregion
    }
}