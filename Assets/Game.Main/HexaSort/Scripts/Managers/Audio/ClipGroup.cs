using System;
using UnityEngine;

namespace HexaSort.Audio
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Clip Group", menuName = "Audio Manager/Clip Group")]
    public class ClipGroup : ScriptableObject
    {
        public AudioClip[] clips;
    }
}