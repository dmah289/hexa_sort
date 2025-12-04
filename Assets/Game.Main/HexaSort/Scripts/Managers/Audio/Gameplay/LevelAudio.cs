using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HexaSort.Audio.Gameplay
{
    public class LevelAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        [SerializeField] private ClipGroup clipGroup;

        // public void OnNotify(LevelState type, object[] args)
        // {
        //     PlaySfx(type);
        // }
        //
        // public void PlaySfx(LevelState type)
        // {
        //     audioSource.clip = clipGroup.clips[(byte)type];
        //     
        //     if(audioSource.isPlaying)
        //         audioSource.Stop();
        //     
        //     audioSource.Play();
        // }
    }
}