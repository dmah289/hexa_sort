using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HexaSort.Audio.Gameplay
{
    public class BlockAudio : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        
        [SerializeField] ClipGroup clipGroup;

        // public void OnNotify(BLockState type, object[] args)
        // {
        //     PlaySfx(type);
        // }
        //
        // private void PlaySfx(BLockState type)
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