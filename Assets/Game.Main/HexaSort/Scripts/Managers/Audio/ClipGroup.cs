using System;
using UnityEngine;

namespace HexaSort.Audio
{
    public enum GameplayAudioType : byte
    {
        CoinCollected = 0,
        PieceMove = 1
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "New Clip Group", menuName = "Audio Manager/Clip Group")]
    public class ClipGroup : ScriptableObject
    {
        public AudioClip[] clips;
    }
}