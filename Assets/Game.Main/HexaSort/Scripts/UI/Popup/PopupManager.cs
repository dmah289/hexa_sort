using System.Collections.Generic;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.InGame;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class PopupManager : MonoSingleton<PopupManager>
    {
        [SerializeField] private WinPanel winPanel;
        [SerializeField] private LoosePanel loosePanel;
        [SerializeField] private List<PopupPanel> popups;
        
        [SerializeField] private UnlockBoosterPanel unlockBoosterPanel;
        [SerializeField] private BuyBoosterPanel buyBoosterPanel;
        [SerializeField] private PopupPanel getMoreLifePanel;

        protected override void Awake()
        {
            base.Awake();
            
            popups = new();
            for (int i = 0; i < transform.childCount; i++)
            {
                PopupPanel panel;
                if (transform.GetChild(i).TryGetComponent(out panel))
                    popups.Add(panel);
            }
        }
        
        public void DisableAllPopups()
        {
            loosePanel.gameObject.SetActive(false);
            winPanel.gameObject.SetActive(false);
            for (int i = 0; i < popups.Count; i++)
            {
                popups[i].gameObject.SetActive(false);
            }
        }

        public void CheckShowUnlockBoosterPopup()
        {
            foreach(var milestone in ConstantKey.BoosterUnlockLevel)
            {
                if (milestone.Value == LocalDataManager.LevelIndex
                    && !LocalDataManager.GetHasShownAndClaimedBoosterTutorial(milestone.Key))
                {
                    unlockBoosterPanel.ShowUnlockBoosterPanel(milestone.Key);
                }
            }
        }
        
        public void ShowBuyBoosterPopup(eBoosterType boosterType)
        {
            buyBoosterPanel.ShowBuyBoosterPanel(boosterType);
        }
        
        public void ShowGetMoreLifePopup()
        {
            getMoreLifePanel.ShowPanel();
        }
    }
}