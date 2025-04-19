using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace _20180713._Scripts
{
    public class EndText : MonoBehaviour
    {
        private Text timerText;

        void Awake()
        {
            timerText = GetComponent<Text>();
        }

        public void SetText(string text)
        {
            timerText.text = text;
        }

        public void SetTextColor(Color color)
        {
            timerText.color = color;
        }
    }
}