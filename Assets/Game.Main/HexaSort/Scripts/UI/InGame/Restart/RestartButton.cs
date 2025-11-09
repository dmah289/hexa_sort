using Cysharp.Threading.Tasks;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.InGame
{
    public class RestartButton : ScaleAnimButton
    {
        [SerializeField] private eRestartButton btnType;
        public UnityEvent<eRestartButton> OnRestartButtonClicked;

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            
            OnRestartButtonClicked?.Invoke(btnType);
        }
    }
}