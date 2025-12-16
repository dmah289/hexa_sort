using System.Collections.Generic;
using HexaSort.UI.Loading.InGame;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        public const int BuyLivePrice = 100;
        public const int RevivePrice = 100;
        public const int WinCoinReward = 50;
        
        public static Dictionary<eBoosterType, int> BoosterPrices = new()
        {
            { eBoosterType.Respawn, 50 },
            { eBoosterType.DestroyStack, 70 }
        };
    }
}