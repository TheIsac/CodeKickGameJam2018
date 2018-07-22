using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace _20180713._Scripts
{
    public class StartGameMusic : MonoBehaviour {

        public FMODUnity.StudioEventEmitter musicEvent;
        public FMODUnity.StudioEventEmitter WindLoop;
        
        void Start()
        {
            WindLoop.enabled = true;
            DontDestroyOnLoad(gameObject);
            musicEvent.SetParameter("StartGame", 1.0f);

        }
    }
}
