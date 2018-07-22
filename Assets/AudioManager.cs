using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace _20180713._Scripts
{
    public class AudioManager : MonoBehaviour
    {
        [FMODUnity.EventRef] public string PickupBlock;
        [FMODUnity.EventRef] public string DropBlock;
        [FMODUnity.EventRef] public string Explosion;
        [FMODUnity.EventRef] public string BlockCollision;
        [FMODUnity.EventRef] public string BlockBuild;
        [FMODUnity.EventRef] public string BombBeep; 
        [FMODUnity.EventRef] public string Thruster;
        [FMODUnity.EventRef] public string PlayerCollision;
        [FMODUnity.EventRef] public string ShipCollision;
        [FMODUnity.EventRef] public string PlayerSwoosh;
        FMOD.Studio.EventInstance soundevent;

        private const float MinSecondsBetweenSameSound = .5f;
        private readonly Dictionary<string, float> lastPlayedSoundByName = new Dictionary<string, float>();

        public void PlaySound(string sound, Vector3 position)
        {
            if (!lastPlayedSoundByName.ContainsKey(sound) ||
                Time.fixedTime - lastPlayedSoundByName[sound] > MinSecondsBetweenSameSound)
            {
                lastPlayedSoundByName[sound] = Time.fixedTime;
                FMODUnity.RuntimeManager.PlayOneShot(sound, position);
            }
        }

        public void ForcePlaySound(string sound, Vector3 position)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sound, position);
        }

        public EventInstance CreateThrusterSoundInstance()
        {
            return FMODUnity.RuntimeManager.CreateInstance(Thruster);
        }
    }
}