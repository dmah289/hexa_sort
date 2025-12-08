using System;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class BoosterManager : MonoBehaviour
    {
        [Header("----- References -----")]
        [SerializeField] private BoosterButton[] boosterButtons;
        
        [Header("---- UI Components ----")]
        [SerializeField] private GridController grid;
        [SerializeField] private TrayController tray;

        private void Awake()
        {
            boosterButtons = GetComponentsInChildren<BoosterButton>();
        }

        public void OnBoosterBtnClicked(eBoosterType boosterType)
        {
            LevelManager.Instance.CurrentLevelState = eLevelState.IsUsingBooster;
            
            switch (boosterType)
            {
                case eBoosterType.Respawn:
                    tray.RespawnCurrentStacks();
                    break;
                case eBoosterType.DestroyStack:
                    
                    break;
            }
        }
        
        public void ShowBoosterButtons()
        {
            for (int i = 0; i < boosterButtons.Length; i++)
                boosterButtons[i].Show();
        }
        
        public void HideBoosterButtons()
        {
            for (int i = 0; i < boosterButtons.Length; i++)
                boosterButtons[i].Hide();
        }
        
        
    }
}