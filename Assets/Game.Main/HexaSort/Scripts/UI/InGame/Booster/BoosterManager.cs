using System;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class BoosterManager : MonoBehaviour
    {
        [Header("----- Self References -----")]
        [SerializeField] private ABoosterButton[] boosterButtons;
        

        private void Awake()
        {
            boosterButtons = GetComponentsInChildren<ABoosterButton>();
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

        public void CheckUnlockBoosterButtons()
        {
            for(int i = 0; i < boosterButtons.Length; i++)
                boosterButtons[i].CheckUnlockBoosterButton();
        }
    }
}