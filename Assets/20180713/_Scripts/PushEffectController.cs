using System;
using UnityEngine;

namespace _20180713._Scripts
{
    public class PushEffectController : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private Vector3 originalScale;
        private float originalAlpha;
        private float progress;
        [SerializeField] private float lengthInSeconds = .8f;
        private float size;
        [SerializeField] private float sizeMultiplier = 4f;
        private bool running;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;

            originalAlpha = meshRenderer.material.color.a;
            originalScale = transform.localScale;
        }

        void Update()
        {
            if (!running) return;
            if (!meshRenderer.enabled)
            {
                meshRenderer.enabled = true;

                var materialColor = meshRenderer.material.color;
                materialColor.a = originalAlpha;
                meshRenderer.material.color = materialColor;
            }

            if (progress < lengthInSeconds)
            {
                progress += Time.deltaTime;
                var progressPercentage = Math.Min(1, progress / lengthInSeconds);
                var newScale = originalScale * size * EaseOutExpo(progressPercentage);
                newScale.z = originalScale.z;
                transform.localScale = newScale;

                var materialColor = meshRenderer.material.color;
                materialColor.a = InverseEaseInExpo(progressPercentage) * originalAlpha;
                meshRenderer.material.color = materialColor;
            }
            else
            {
                running = false;
                progress = 0;
                meshRenderer.enabled = false;
            }
        }

        public void Run(float endSize)
        {
            size = endSize * sizeMultiplier;
            progress = 0;
            running = true;
        }

        private float EaseOutExpo(float curveProgress)
        {
            return -Mathf.Pow(2, -10 * curveProgress) + 1;
        }

        private float InverseEaseInExpo(float curveProgress)
        {
            var inverseEaseInExpo = -1 * Mathf.Pow(2, 10 * (curveProgress - 1)) + 1;
            return inverseEaseInExpo;
        }
    }
}