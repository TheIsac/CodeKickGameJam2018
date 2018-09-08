using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _20180713._Scripts
{
    public class IntegerOptionsSlider : SettingsSlider<int>
    {
        public List<string> OptionNames;
        public List<int> OptionValues;

        public IntegerOptionsSlider()
        {
            IndexToTextAndValueMap = new Dictionary<int, KeyValuePair<string, int>>();
        }

        private new void Start()
        {
            for (var i = 0; i < OptionNames.Count; i++)
            {
                IndexToTextAndValueMap[i] = new KeyValuePair<string, int>(OptionNames[i], OptionValues[i]);
            }

            var slider = GetComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = OptionNames.Count - 1;

            base.Start();
        }
    }
}