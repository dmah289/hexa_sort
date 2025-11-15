using Cysharp.Threading.Tasks;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.InGame
{
    public class ExitButton : ScaleAnimButton
    {
        [SerializeField] private eGiveUpButton btnType;
        public UnityEvent<eGiveUpButton> OnExitButtonClicked;

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            
            OnExitButtonClicked?.Invoke(btnType);
        }
    }
}