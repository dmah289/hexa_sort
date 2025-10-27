using UnityEngine;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        #region Grid

        public const float BOARD_CELL_R = 0.99f / 2;
        public const float BOARD_CELL_r = 0.9f / 2;
        public const float BOARD_CELL_THICKNESS = 0.12f;
        public static readonly Vector2 CELL_SPACING = new (0.03f, 0.03f);
        
        public const int MAX_CELL_PER_ROW = 12;

        #endregion

        #region Tray

        public const float TRAY_THICKNESS = 0.06f;
        public const float TRAY_WIDTH = 1.61f * 2f;

        public const float SPAWN_POS_OFFSET_X = 3f;
        public const float HEX_STACK_SPACING = 1.05f;
        public const float SLIDE_IN_VELOCITY = 10f;
        public const float SNAP_TO_TARGET_VELOCITY = 18f;

        #endregion
        
        #region Stack & Pieces
        
        public const float HEX_PIECE_THICKNESS = 0.12f;
        public static float INITIAL_PIECE_SCALE = 0.95f;

        #endregion
    }
}