using UnityEngine;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        public const float STACK_LIFTING_OFFSET_Z = -3.5f;

        public static Vector3 STACK_LOCAL_POS_ON_CELL = new (0, 0, -2f * BOARD_CELL_THICKNESS + 0.01f);
    }
}