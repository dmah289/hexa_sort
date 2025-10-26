using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        #region Camera

        public const float ZOOM_IN_OFFSET_ORTHO = 3f;
        
        public static float GAMEPLAY_CAM_TO_PLANE_DISTANCE = 10f;
        public static float GAMEPLAY_CAM_X_EULER = 20f;
        public static float GAMEPLAY_CAM_Y_OFFSET = GAMEPLAY_CAM_TO_PLANE_DISTANCE * (float)Math.Tan(Mathf.Deg2Rad * (GAMEPLAY_CAM_X_EULER + 0.5f));

        #endregion
        
    }
}