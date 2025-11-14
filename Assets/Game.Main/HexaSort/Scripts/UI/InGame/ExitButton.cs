using Cysharp.Threading.Tasks;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.InGame
{
    public class ExitButton : ScaleAnimButton
    {
        [SerializeField] private eGiveUpButton btnType;
        public UnityEvent<eGiveUpButton> OnRestartButtonClicked;

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            
            OnRestartButtonClicked?.Invoke(btnType);
        }
    }
}