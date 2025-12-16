using Framework.UI;
using HexaSort.UI.Loading.InGame;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class UnlockBoosterPanel : PopupPanel
    {
        [SerializeField] private UnlockBoosterPopup unlockBoosterPopup;
        
        public void ShowUnlockBoosterPanel(eBoosterType boosterType)
        {
            ShowPanel();
            
            unlockBoosterPopup.SetupBoosterType(boosterType);
        }
    }
}