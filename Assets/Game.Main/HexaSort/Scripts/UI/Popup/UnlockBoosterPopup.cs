using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.InGame;
using HexaSort.UI.Loading.MainMenu.Home;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class UnlockBoosterPopup : APopup
    {
        [SerializeField] private eBoosterType _boosterType;
        
        [SerializeField] private GameObject respawnGroup;
        [SerializeField] private GameObject destroyStackGroup;

        protected override void Awake()
        {
            base.Awake();

            _boosterType = eBoosterType.None;
        }

        public void SetupBoosterType(eBoosterType boosterType)
        {
            _boosterType = boosterType;
            respawnGroup.SetActive(false);
            destroyStackGroup.SetActive(false);
            
            if (_boosterType == eBoosterType.Respawn)
            {
                respawnGroup.SetActive(true);
                LocalDataManager.HasShownAndClaimedRespawnBoosterTutorial = true;
            }
            else if (_boosterType == eBoosterType.DestroyStack)
            {
                destroyStackGroup.SetActive(true);
                LocalDataManager.HasShownAndClaimedDestroyBoosterTutorial = true;
            }
        }

        protected override UniTaskVoid HidePopupAsync()
        {
            switch (_boosterType)
            {
                case eBoosterType.Respawn:
                    LocalDataManager.BoosterRespawnAmount++;
                    break;
                case eBoosterType.DestroyStack:
                    LocalDataManager.BoosterDestroyStackAmount++;
                    break;
            }
            _boosterType = eBoosterType.None;
            
            return base.HidePopupAsync();
        }
    }
}