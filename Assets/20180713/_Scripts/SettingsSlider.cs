using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(Slider))]
    public class SettingsSlider<T> : MonoBehaviour
    {
        public Text SliderValueText;
        protected string SliderValueTextSuffix = "";

        protected Dictionary<int, KeyValuePair<string, T>> IndexToTextAndValueMap =
            new Dictionary<int, KeyValuePair<string, T>>();

        private readonly List<Action<T>> listeners = new List<Action<T>>();

        public void Start()
        {
            var slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnValueChanged);

            slider.value = slider.minValue;
            var value = (int) slider.value;
            if (IndexToTextAndValueMap.ContainsKey(value))
            {
                var textAndValue = IndexToTextAndValueMap[value];
                SetSliderValueText(textAndValue.Key);
            }
        }

        public void Reset()
        {
            var slider = GetComponent<Slider>();
            slider.value = slider.minValue;
        }

        public void AddListener(Action<T> onValueChanged)
        {
            listeners.Add(onValueChanged);
        }

        protected void RefreshSliderValueText()
        {
            var slider = GetComponent<Slider>();
            OnValueChanged(slider.value);
        }
        
        private void OnValueChanged(float newValue)
        {
            var newCount = (int) newValue;
            if (IndexToTextAndValueMap.ContainsKey(newCount))
            {
                var textAndValue = IndexToTextAndValueMap[newCount];
                SetSliderValueText(textAndValue.Key);
                listeners.ForEach(callback => callback(textAndValue.Value));
            }
        }

        private void SetSliderValueText(string text)
        {
            SliderValueText.text = text + SliderValueTextSuffix;
        }
    }
}