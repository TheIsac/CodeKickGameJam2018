using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace _20180713._Scripts
{
    public class AudioManager : MonoBehaviour
    {
        [FMODUnity.EventRef] public string pickupBlock;
        [FMODUnity.EventRef] public string dropBlock;
        [FMODUnity.EventRef] public string explosion;
        [FMODUnity.EventRef] public string blockCollision;
        [FMODUnity.EventRef] public string blockBuild;
        [FMODUnity.EventRef] public string bombBeep;
        [FMODUnity.EventRef] public string thruster;
        FMOD.Studio.EventInstance soundevent;

        void Start()
        {
            //soundevent = FMODUnity.RuntimeManager.CreateInstance(bombBeep);
        }

        void Update()
        {
            //FMODUnity.RuntimeManager.AttachInstanceToGameObject(bombBeep, )
        }

        public void PlaySound(string sound, UnityEngine.Vector3 position)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sound, position);
        }

        public EventInstance CreateThrusterSoundInstance()
        {
            return FMODUnity.RuntimeManager.CreateInstance(thruster);
        }
    }
}