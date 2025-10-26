namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        #region Grid

        public const float BOARD_CELL_R = 0.99f / 2;
        public const float BOARD_CELL_r = 0.9f / 2;
        public const float BOARD_CELL_HEIGHT = 0.06f;
        
        public const int MAX_CELL_PER_ROW = 12;

        #endregion

        #region Tray

        public const float TRAY_HEIHGT = 0.06f;
        public const float TRAY_WIDTH = 1.61f * 2f;

        public const float SPAWN_POS_OFFSET_X = 3f;
        public const float HEX_STACK_SPACING = 1.05f;
        public const float SLIDE_IN_VELOCITY = 10f;
        public const float BACK_TO_HOLDER_VELOCITY = 8f;

        #endregion
        
        #region Stack & Pieces
        
        public const float HEX_PIECE_THICKNESS = 0.12f;
        
        #endregion
    }
}