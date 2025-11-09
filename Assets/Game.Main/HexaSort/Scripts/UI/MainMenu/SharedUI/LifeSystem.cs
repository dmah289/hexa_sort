using System;
using DG.Tweening;
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
        private const int MAX_LIVES = 5;
        private static string MaxLifeKey = "MAX";
        
        [Header("Self Components")]
        [SerializeField] private Text counter;
        [SerializeField] private Text timer;

        [Header("Settings")] 
        [SerializeField] private float targetScale;
        [SerializeField] private float animDuration;

        
        [Header("Life System Config")]
        [SerializeField] private int curLife;
        private float countdownRemaining;
        private DateTime lastSaveTime;
        [SerializeField] private bool isTimerRunning;
        [SerializeField] private float timeForOneLife;
        
        public int CurLife
        {
            get => curLife;
            set
            {
                curLife = Mathf.Clamp(value, 0, MAX_LIVES);
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

        #region Unity APIs

        private void Awake()
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
        
        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt(ConstantKey.CurLifeKey, curLife);
            PlayerPrefs.SetString(ConstantKey.LastSaveTimeKey, DateTime.Now.ToString());
            PlayerPrefs.SetFloat(ConstantKey.LastCountdownRemainingKey, countdownRemaining);
            PlayerPrefs.Save();
        }

        #endregion

        #region Class Methods

        private void LoadLifeData()
        {
            CurLife = 5;
            CurCountdown = 0;
            if (PlayerPrefs.HasKey(ConstantKey.LastSaveTimeKey))
            {
                DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString(ConstantKey.LastSaveTimeKey));
                TimeSpan elapsedTime = DateTime.Now - savedTime;
                countdownRemaining = PlayerPrefs.GetFloat(ConstantKey.LastCountdownRemainingKey);
                
                float totalSecondsElapsed = (float)elapsedTime.TotalSeconds;
                int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);

                CurLife = PlayerPrefs.GetInt(ConstantKey.CurLifeKey);
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
            counter.text = $"{curLife}";

            if (curLife == MAX_LIVES)
            {
                isTimerRunning = false;
                timer.text = MaxLifeKey;
            }
            else isTimerRunning = true;
        }

        private void UpdateTimerDisplay()
        {
            if (curLife < MAX_LIVES)
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

        #endregion
        
        #region Life Changed Event Bus

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