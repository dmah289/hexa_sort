using System.Collections.Generic;
using HexaSort.UI.Loading.InGame;

namespace manhnd_sdk.Scripts.ConstantKeyNamespace
{
    public static partial class ConstantKey
    {
        public const int BuyLivePrice = 25;
        public const int RevivePrice = 20;
        public const int WinCoinReward = 50;
        
        public static Dictionary<eBoosterType, int> BoosterPrices = new()
        {
            { eBoosterType.Respawn, 15 },
            { eBoosterType.DestroyStack, 15 }
        };
    }
}