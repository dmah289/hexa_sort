using HexaSort.UI.Gameplay.Goals;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Audio.Gameplay
{
    public class PieceAudio : MonoBehaviour
    {
        [SerializeField] private ClipGroup clipGroup;
        
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            // EventBus<GoalCollectedDTO>.Register(onEventWithArgs: On);
        }

        // public void PlaySfx(PieceState type)
        // {
        //     audioSource.clip = clipGroup.clips[(byte)type];
        //
        //     if (audioSource.isPlaying)
        //         audioSource.Stop();
        //     
        //     audioSource.Play();
        // }
    }
}