using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace _20180713._Scripts
{
    public class BlockEffectController : MonoBehaviour
    {
        private Material material;
        private bool selected;
        private float damage;

        void Start()
        {
            material = GetComponent<Renderer>().material;
            material.SetColor("_EmissionColor", Color.black);
        }

        public void SetDamage(float factor)
        {
            damage = factor;
            UpdateEmissionColor();
        }

        public void SetSelected(bool selectedChange)
        {
            selected = selectedChange;
            UpdateEmissionColor();
        }

        public bool IsSelected()
        {
            return selected;
        }

        private void UpdateEmissionColor()
        {
            var currentColor = material.GetColor("_EmissionColor");
            currentColor.r = EaseOutCubic(0, 1, damage);
            currentColor.g = 0;
            currentColor.b = selected ? .5f : 0;
            material.SetColor("_EmissionColor", currentColor);
        }
        
        private static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }
    }
}