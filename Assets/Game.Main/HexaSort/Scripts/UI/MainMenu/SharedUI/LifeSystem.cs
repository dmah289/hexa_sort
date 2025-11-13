using System;
using System.Globalization;
using DG.Tweening;
using Game.Main.HexaSort.Scripts.Managers;
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
    
    public class LifeSystem : MonoBehaviour, IEventBusListener
    {
        public const int MAX_LIVES = 5;
        private static string MaxLifeKey = "MAX";
        
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
        
        public int CurLife
        {
            get => LocalDataManager.CurrentLife;
            set
            {
                LocalDataManager.CurrentLife = Mathf.Clamp(value, 0, MAX_LIVES);
                UpdateLifeCounterDisplay();
            }
        }
        
        public float CurCountdown
        {
            get => countdownRemaining;
            set
            {
                countdownRemaining = Mathf.Max(value, 0);
                UpdateTimerDisplay();
            }
        }
        
        public bool CanPlay => CurLife > 0;
        
        public RectTransform TargetRectTransform => counter.rectTransform;

        #region Unity APIs

        private void Awake()
        {
            RegisterCallbacks();
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

                if (countdownRemaining <= 0)
                {
                    CurLife++;
                    if(CurLife < MAX_LIVES) CurCountdown = timeForOneLife;
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
                CurLife = MAX_LIVES;
                CurCountdown = 0;
            }
            else
            {
                DateTime savedTime = DateTime.Parse(LocalDataManager.LastLifeSaveTime);
                TimeSpan elapsedTime = DateTime.Now - savedTime;
                countdownRemaining = LocalDataManager.LastCountdownRemaining;
                
                float totalSecondsElapsed = (float)elapsedTime.TotalSeconds;
                int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);

                CurLife = LocalDataManager.CurrentLife;
                if (livesToAdd > 0)
                {
                    CurLife += livesToAdd;
                    CurCountdown = timeForOneLife - totalSecondsElapsed % timeForOneLife;
                }
                else
                {
                    CurCountdown -= totalSecondsElapsed;
                }
            }
        }

        private void UpdateLifeCounterDisplay()
        {
            counter.text = $"{LocalDataManager.CurrentLife}";

            if (LocalDataManager.CurrentLife == MAX_LIVES)
            {
                isTimerRunning = false;
                timer.text = MaxLifeKey;
            }
            else isTimerRunning = true;
        }

        private void UpdateTimerDisplay()
        {
            if (LocalDataManager.CurrentLife < MAX_LIVES)
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

        #endregion
        
        #region Event Bus

        public void RegisterCallbacks()
        {
            EventBus<LifeChangedEventDTO>.Register(onEventWithArgs: OnLifeChanged);
        }
        
        public void OnLifeChanged(LifeChangedEventDTO dto)
        {
            CurLife += dto.amount;
            
            if(dto.amount < 0) CurCountdown = timeForOneLife;
        }

        public void DeregisterCallbacks()
        {
            EventBus<LifeChangedEventDTO>.Deregister(onEventWithArgs: OnLifeChanged);
        }

        #endregion
    }
}