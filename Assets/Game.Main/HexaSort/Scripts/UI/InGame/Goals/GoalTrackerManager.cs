using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Main.LevelEditor.Scripts.LevelData;
using manhnd_sdk.ExtensionMethods;
using HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Gameplay.Goals
{
    public struct eGoalCollectedDTO : IEventDTO
    {
        public eLevelGoalType goalType;
        public int collectedAmount;
        
        public eGoalCollectedDTO(eLevelGoalType goalType, int collectedAmount)
        {
            this.goalType = goalType;
            this.collectedAmount = collectedAmount;
        }
    }
    
    public class GoalTrackerManager : MonoBehaviour, IEventBusListener
    {
        private const float SlideSpeed = 850f;
        
        [Header("Self Components")]
        [SerializeField] private RectTransform containerRt;
        
        [Header("References")]
        [SerializeField] private RectTransform panelRt;
        [SerializeField] private TextMeshProUGUI titleTxt;
        public GoalTrackerPanel[] goalTrackerPanels;

        #region Unity APIs

        private void Awake()
        {
            goalTrackerPanels = GetComponentsInChildren<GoalTrackerPanel>();
            RegisterCallbacks();
        }

        private void OnEnable()
        {
            PlayStartLevelAnim().Forget();
        }

        private async UniTask PlayStartLevelAnim()
        {
            SetLevelGoalData();
            await ResetUIStates();
            await AnimateSlidingIn();
        }

        #endregion

        #region Class Methods

        private async UniTask ResetUIStates()
        {
            panelRt.GetComponent<CanvasGroup>().alpha = 0;
            
            await UniTask.DelayFrame(1);
            
            panelRt.anchoredPosition = new Vector2((int)(Screen.width / 2) + panelRt.rect.width / 2f + 155f,
                (int)(-Screen.height / 2) + panelRt.rect.height / 2 + 31f);
           //Debug.Log(panelRt.anchoredPosition);
            panelRt.GetComponent<Image>().SetAlpha(1);
            
            titleTxt.gameObject.SetActive(true);
            titleTxt.alpha = 1;
            titleTxt.transform.localScale = Vector3.one;
            
            containerRt.GetComponent<Image>().SetAlpha(1);
            
            await UniTask.DelayFrame(1);
            
            panelRt.GetComponent<CanvasGroup>().alpha = 1;
        }

        private async UniTask AnimateSlidingIn()
        {
            await panelRt.DOAnchorPosX(0, panelRt.anchoredPosition.x / 900f)
                .SetEase(Ease.OutBack)
                .ToUniTask();
            
            await UniTask.Delay(1000);
            
            float duration = Mathf.Abs(panelRt.anchoredPosition.y) / SlideSpeed;
            
            panelRt.DOAnchorPosY(0, duration);
            panelRt.GetComponent<Image>().DOFade(0, duration);
            
            titleTxt.DOFade(0, duration);
            titleTxt.transform.DOScale(0, 0.2f * duration).OnComplete(() =>
            {
                titleTxt.gameObject.SetActive(false);
            });

            if (LevelManager.Instance.LevelGoalCount <= 1)
                containerRt.GetComponent<Image>().DOFade(0, duration);
        }

        private void SetLevelGoalData()
        {
            // TODO : Set from Level Data
            
            Canvas.ForceUpdateCanvases();
        }

        #endregion

        #region On Goal Collected Listeners

        public void RegisterCallbacks()
        {
            EventBus<eGoalCollectedDTO>.Register(onEventWithArgs: OnGoalCollected);
        }

        private void OnGoalCollected(eGoalCollectedDTO dto)
        {
            goalTrackerPanels[(int)dto.goalType].OnGoalCollected(dto.collectedAmount);
        }

        public void DeregisterCallbacks()
        {
            EventBus<eGoalCollectedDTO>.Deregister(onEventWithArgs: OnGoalCollected);
        }

        #endregion

        
    }
}