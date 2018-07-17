using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace _20180713._Scripts
{
    public class GameTimer : MonoBehaviour
    {
        private Text timerText;

        void Awake()
        {
            timerText = GetComponent<Text>();
        }

        public void SetTime(float time)
        {
            var minutesLeft = (int) Math.Floor(time / 60);
            var secondsLeft = (int) (time % 60);
            timerText.text = PadInt(minutesLeft) + ":" + PadInt(secondsLeft);
        }

        private static string PadInt(int time)
        {
            return time >= 10 ? time.ToString() : "0" + time;
        }
    }
}