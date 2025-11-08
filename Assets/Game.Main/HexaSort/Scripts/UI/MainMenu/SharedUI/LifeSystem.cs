using System;
using DG.Tweening;
using Framework;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public class LifeSystem : MonoSingleton<LifeSystem>
    {
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
        [SerializeField] private float timeForOneLife = 10;
        private int maxLives = 5;
        
        
        public int CurLife
        {
            get => curLife;
            set
            {
                curLife = Mathf.Clamp(value, 0, maxLives);
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

        protected override void Awake()
        {
            base.Awake();
            
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
                    if(CurLife < maxLives) CurCountdown = timeForOneLife;
                }
            }
        }
        
        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt(KeySave.curLifeKey, curLife);
            PlayerPrefs.SetString(KeySave.lastSaveTimeKey, DateTime.Now.ToString());
            PlayerPrefs.SetFloat(KeySave.lastCountdownRemaining, countdownRemaining);
            PlayerPrefs.Save();
        }
        
        private void LoadLifeData()
        {
            CurLife = 5;
            CurCountdown = 0;
            if (PlayerPrefs.HasKey(KeySave.lastSaveTimeKey))
            {
                DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString(KeySave.lastSaveTimeKey));
                TimeSpan elapsedTime = DateTime.Now - savedTime;
                countdownRemaining = PlayerPrefs.GetFloat(KeySave.lastCountdownRemaining);
                
                float totalSecondsElapsed = (float)elapsedTime.TotalSeconds;
                int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);

                CurLife = PlayerPrefs.GetInt(KeySave.curLifeKey);
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

            if (curLife == maxLives)
            {
                isTimerRunning = false;
                timer.text = KeySave.maxKey;
            }
            else isTimerRunning = true;
        }

        private void UpdateTimerDisplay()
        {
            if (curLife < maxLives)
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

        public void DecreaseOnLifeLost()
        {
            CurLife--;
            CurCountdown = timeForOneLife;
        }
    }
}