using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy.Audio
{
    /// <summary>
    /// System responsible for playing sounds. 
    /// TODO: Redo this system properly.
    /// </summary>
    public class AudioSystem : MonoBehaviour
    {
        private static AudioSystem instance;


        [SerializeField]
        private AudioSource audioSourceSFX;
        [SerializeField]
        private AudioSource audioSourceBGM;

        [SerializeField]
        private AudioClip OnCandySwap;

        [SerializeField]
        private AudioClip onScoreIncrease;

        [SerializeField]
        private AudioClip backgroundMusic;

        private void Awake()
        {
            instance = this;
        }

        public static void PlayOnCandySwap()
        {
            instance.audioSourceSFX.PlayOneShot(instance.OnCandySwap);
        }

        public static void PlayOnScoreIncrease()
        {
            instance.audioSourceSFX.PlayOneShot(instance.onScoreIncrease);
        }


        public static void PlayBackgroundMusic()
        {
            instance.audioSourceBGM.clip = instance.backgroundMusic;
            instance.audioSourceBGM.Play();
        }


    }

}
