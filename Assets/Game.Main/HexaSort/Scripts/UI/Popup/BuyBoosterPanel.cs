using Framework.UI;
using HexaSort.UI.Loading.InGame;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class BuyBoosterPanel : PopupPanel
    {
        [SerializeField] private BuyBoosterPopup buyBoosterPopup;
        
        public void ShowBuyBoosterPanel(eBoosterType boosterType)
        {
            ShowPanel();
            
            buyBoosterPopup.SetupBoosterType(boosterType);
            buyBoosterPopup.ShowPopup();
        }
    }
}