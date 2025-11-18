using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LevelEditor.LevelData;
using manhnd_sdk.ExtensionMethods;
using HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Gameplay.Goals
{
    public struct GoalCollectedDTO : IEventDTO
    {
        public eLevelGoalType goalType;
        public int collectedAmount;
        
        public GoalCollectedDTO(eLevelGoalType goalType, int collectedAmount)
        {
            this.goalType = goalType;
            this.collectedAmount = collectedAmount;
        }
    }
    
    public struct TotalGoalCollectedDTO : IEventDTO
    {
        public eLevelGoalType goalType;
        public int totalCollectedAmount;
        
        public TotalGoalCollectedDTO(eLevelGoalType goalType, int totalCollectedAmount)
        {
            this.goalType = goalType;
            this.totalCollectedAmount = totalCollectedAmount;
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

        public async UniTask PlayStartLevelAnim(LevelGoalData[] goalData)
        {
            SetLevelGoalData(goalData);
            await ResetUIStates();
            await AnimateSlidingIn();
        }

        #endregion

        #region Class Methods

        private async UniTask ResetUIStates()
        {
            panelRt.GetComponent<CanvasGroup>().alpha = 0;
            
            await UniTask.DelayFrame(3);
            
            panelRt.anchoredPosition = new Vector2((int)(Screen.width / 2) + panelRt.rect.width / 2f + 155f,
                (int)(-Screen.height / 2) + panelRt.rect.height / 2 + 31f);
            panelRt.sizeDelta = new Vector2(1000f, panelRt.sizeDelta.y);
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
            panelRt.DOSizeDelta(new Vector2(containerRt.sizeDelta.x, panelRt.anchoredPosition.y), duration);
            
            // Piece Bg Collapse Animation
            PieceTrackerPanel piecePanel = goalTrackerPanels[0] as PieceTrackerPanel;
            piecePanel?.AnimateExpansion(duration);
            
            titleTxt.DOFade(0, duration);
            titleTxt.transform.DOScale(0, 0.2f * duration).OnComplete(() =>
            {
                titleTxt.gameObject.SetActive(false);
            });

            if (LevelManager.Instance.LevelGoalCount <= 1)
                containerRt.GetComponent<Image>().DOFade(0, duration);
        }

        private void SetLevelGoalData(LevelGoalData[] goalData)
        {
            // TODO : Set from Level Data
            Array.Sort(goalData, (a, b) => a.type.CompareTo(b.type));
            
            for (int i = 0; i < goalData.Length; i++)
            {
                goalTrackerPanels[i].SetUp(goalData[i]);
            }
            
            Canvas.ForceUpdateCanvases();
        }

        #endregion

        #region On Goal Collected Listeners

        public void RegisterCallbacks()
        {
            EventBus<GoalCollectedDTO>.Register(onEventWithArgs: OnGoalCollected);
        }

        private void OnGoalCollected(GoalCollectedDTO dto)
        {
            goalTrackerPanels[(int)dto.goalType].OnGoalCollected(dto.collectedAmount);
        }

        public void DeregisterCallbacks()
        {
            EventBus<GoalCollectedDTO>.Deregister(onEventWithArgs: OnGoalCollected);
        }

        #endregion
        
    }
}