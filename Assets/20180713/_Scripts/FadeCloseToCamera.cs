using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCloseToCamera : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color color;
    private float fadeStartDistace = 50;

    private const float FadeInSeconds = 5;
    private float fadeInProgress;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        color = objectRenderer.material.color;
        objectRenderer.material.color = new Color(color.r, color.g, color.b, 0);
    }

    void Update()
    {
        var distance = Math.Max(0, -1 * transform.position.y);
        if (fadeInProgress < FadeInSeconds)
        {
            fadeInProgress += Time.deltaTime;
            var alpha = Math.Min(1, fadeInProgress / FadeInSeconds);
            objectRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }
        else if (distance < fadeStartDistace)
        {
            var progress = Math.Max(0, (distance - 6) / (fadeStartDistace - 6));
//            var alpha = Mathf.Pow(progress, 2);
            var easeInCirc = EaseInCirc(0, 1, progress);
//            Debug.Log("ease cloud " + easeInCirc);
            var alpha = Math.Max(0.01f, easeInCirc);
            objectRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }
    }

    private float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    private float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    private float EaseInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value + start;
        value -= 2;
        return -end * 0.5f * (value * value * value * value - 2) + start;
    }
}