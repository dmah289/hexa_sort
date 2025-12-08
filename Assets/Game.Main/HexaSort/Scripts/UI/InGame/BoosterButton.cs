using DG.Tweening;
using Framework.UI;
using manhnd_sdk.Scripts.ExtensionMethods;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public enum eBoosterType
    {
        Respawn,
        DestroyStack
    }
    
    public class BoosterButton : ScaleAnimButton
    {
        [Header("----- Self References -----")]
        
        [Header("----- State Managers -----")]
        [SerializeField] private eBoosterType boosterType;
        [SerializeField] private float shownPosX;

        public void Show()      
        {
            selfRT.anchoredPosition = selfRT.anchoredPosition.With(x: -shownPosX);
            selfRT.DOLocalMoveX(shownPosX, 0.2f)
                .SetEase(Ease.OutBack);         
        }

        public void Hide()
        {
            selfRT.anchoredPosition = selfRT.anchoredPosition.With(x: shownPosX);
            selfRT.DOLocalMoveX(-shownPosX, 0.2f)
                .SetEase(Ease.InBack);
        }
    }
}