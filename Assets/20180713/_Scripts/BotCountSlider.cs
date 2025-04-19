using UnityEngine.UI;

namespace _20180713._Scripts
{
    public class BotCountSlider : IntegerOptionsSlider
    {
        public IntegerOptionsSlider PlayerCountSlider;

        public BotCountSlider()
        {
            SliderValueTextSuffix = " / 1";
        }

        private new void Start()
        {
            base.Start();
            PlayerCountSlider.AddListener(OnPlayerCountChanged);
            OnPlayerCountChanged(1);
        }

        private void OnPlayerCountChanged(int newPlayerCount)
        {
            var slider = GetComponent<Slider>();
            slider.maxValue = newPlayerCount;
            if (slider.value > newPlayerCount) slider.value = newPlayerCount;

            SliderValueTextSuffix = " / " + newPlayerCount;
            RefreshSliderValueText();
        }
    }
}