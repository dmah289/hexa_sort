using System.Collections.Generic;
using Framework.UI;
using HexaSort.UI.Loading.InGame;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class PopupManager : MonoSingleton<PopupManager>
    {
        [SerializeField] private WinPanel winPanel;
        [SerializeField] private LoosePanel loosePanel;
        [SerializeField] private PopupPanel unlockBoosterPanel;
        [SerializeField] private List<PopupPanel> popups;

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
            for (int i = 0; i < popups.Count; i++)
            {
                popups[i].gameObject.SetActive(false);
            }
        }

        public void ShowUnlockBoosterPanel()
        {
            unlockBoosterPanel.ShowPanel();
        }
    }
}