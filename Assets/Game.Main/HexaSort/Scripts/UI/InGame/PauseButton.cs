using Framework.UI;
using HexaSort.Scripts.Core.Controllers;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class PauseButton : ScaleAnimButton
    {
        [Header("References")]
        [SerializeField] private MergeController mergeController;
        
        protected override void OnButtonClicked()
        {
            if (!mergeController.IsCheckingMergeSequence)
            {
                base.OnButtonClicked();
            }
        }
    }
}