using Framework.UI;
using HexaSort.Scripts.Core.Controllers;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class PauseButton : ScaleAnimButton
    {
        protected override void OnButtonClicked()
        {
            if (!MergeController.Instance.IsCheckingMergeSequence)
            {
                base.OnButtonClicked();
            }
        }
    }
}