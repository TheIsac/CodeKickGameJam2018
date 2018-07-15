using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _20180713._Scripts
{
    public class SoundOneshot : MonoBehaviour
    {

        [FMODUnity.EventRef]
        public string pickupBlock;
        [FMODUnity.EventRef]
        public string dropBlock;
        [FMODUnity.EventRef]
        public string explosion;
        [FMODUnity.EventRef]
        public string blockCollision;
        [FMODUnity.EventRef]
        public string bombBeep;
        FMOD.Studio.EventInstance soundevent;



        void Start()
        {
            soundevent = FMODUnity.RuntimeManager.CreateInstance(bombBeep);
        }

        void Update()
        {
            //FMODUnity.RuntimeManager.AttachInstanceToGameObject(bombBeep, )
        }
        public void PlaySound(string sound, UnityEngine.Vector3 position)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sound, position);
        }
    }
}