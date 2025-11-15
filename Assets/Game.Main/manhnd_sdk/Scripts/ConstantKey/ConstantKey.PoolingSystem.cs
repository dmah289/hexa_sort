using System;
using System.Collections.Generic;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        public const int MAX_POOL_SIZE = 155;
        public const int INITIAL_POOL_SIZE = 3;
        public static readonly int POOL_AMOUNT = Enum.GetValues(typeof(PoolingType)).Length;
        public static readonly string GAMEPLAY_POOL_PARENT_TAG = "gameplay_pools";
        public static readonly string UI_POOL_PARENT_TAG = "ui_pools";
        
        public static readonly Dictionary<PoolingType, string> ADRESSABLE_POOLING_KEY = new()
        {
            {PoolingType.HexCell, "pooling_hex_cell"},
            {PoolingType.HexStack, "pooling_hex_stack"},
            {PoolingType.HexPiece, "pooling_hex_piece"},
            {PoolingType.Toast, "pooling_toast"},
            {PoolingType.CoinFly, "pooling_coin_fly"},
            {PoolingType.LifeFly, "pooling_life_fly"},
            {PoolingType.StarTrail, "pooling_star_trail"},
        };
        
        
    }
}