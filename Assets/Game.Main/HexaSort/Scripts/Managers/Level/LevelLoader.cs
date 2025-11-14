using System;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Entities;
using manhnd_sdk.Scripts.Helpers;
using UnityEngine;

namespace HexaSort.Scripts.Managers
{
    public class LevelLoader : MonoBehaviour
    {
        public void SetupLevel(GridController grid)
        {
            grid.SetupLevel();
        }

        public void CleanUpLevel(GridController grid, TrayController tray)
        {
            grid.CleanUp();
            tray.CleanUp();
        }
    }
}