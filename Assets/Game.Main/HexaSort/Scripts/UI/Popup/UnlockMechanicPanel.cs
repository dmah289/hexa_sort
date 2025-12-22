using Framework.UI;
using HexaSort.UI.Loading.InGame;
using LevelEditor.LevelData;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class UnlockMechanicPanel : PopupPanel
    {
        [SerializeField] private UnlockMechanicPopup unlockMechanicPopup;
        
        public void ShowUnlockMechanicPanel(eMechanicsType mechanicType)
        {
            ShowPanel();
            
            unlockMechanicPopup.SetupMechanicType(mechanicType);
            unlockMechanicPopup.ShowPopup();
        }
    }
}