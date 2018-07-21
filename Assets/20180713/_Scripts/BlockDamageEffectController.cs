using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockDamageEffectController : MonoBehaviour
    {
        private Material material;

        void Start()
        {
            material = GetComponent<Renderer>().material;
            material.SetColor("_EmissionColor", Color.black);
        }

        public void SetDamage(float factor)
        {
            var currentColor = material.GetColor("_EmissionColor");
            currentColor.r = EaseInCubic(0, 1, factor);
            currentColor.g = 0;
            currentColor.b = 0;
            material.SetColor("_EmissionColor", currentColor);
        }

        private float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }
    }
}