using Framework.UI;
using HexaSort.Managers.Level;
using HexaSort.Scripts.Core.Controllers;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class PauseButton : MButton
    {
        protected override void OnButtonClicked()
        {
            if (MergeController.Instance.IsCheckingMergeSequence 
                || LevelManager.Instance.CurrentLevelState != eLevelState.Playing)
                return;
            
            base.OnButtonClicked();
        }
    }
}