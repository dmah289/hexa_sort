using Cysharp.Threading.Tasks;
using Framework.UI;
using HexaSort.Managers.Level;
using HexaSort.Scripts.Core.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.InGame
{
    public class ExitButton : MButton
    {
        [SerializeField] private eGiveUpButton btnType;
        public UnityEvent<eGiveUpButton> OnExitButtonClicked;

        protected override void OnButtonClicked()
        {
            if (MergeController.Instance.IsCheckingMergeSequence 
                || LevelManager.Instance.CurrentLevelState != eLevelState.Playing)
                return;
            
            base.OnButtonClicked();
            
            OnExitButtonClicked?.Invoke(btnType);
        }
    }
}