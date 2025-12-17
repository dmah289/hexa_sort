using System;
using System.Collections.Generic;
using HexaSort.UI.Loading.InGame;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        public const int MAX_LEVEL = 2;

        #region Camera

        public const float ZOOM_IN_OFFSET_ORTHO = 3f;
        
        public static float GAMEPLAY_CAM_TO_PLANE_DISTANCE = 10f;
        public static float GAMEPLAY_CAM_X_EULER = 20f;
        public static float GAMEPLAY_CAM_Y_OFFSET = GAMEPLAY_CAM_TO_PLANE_DISTANCE * (float)Math.Tan(Mathf.Deg2Rad * (GAMEPLAY_CAM_X_EULER + 0.5f));

        #endregion

        public const string LevelCurveVersion = "level_curve_default";
        
        public const int MaxDestroyableStackOnLoose = 2;
        public const float BottomRevivePanelOffsetY = -300f;
        
        public static Dictionary<eBoosterType, int> BoosterUnlockLevel = new()
        {
            { eBoosterType.Respawn, 1 },
            { eBoosterType.DestroyStack, 1 }
        };
    }
}