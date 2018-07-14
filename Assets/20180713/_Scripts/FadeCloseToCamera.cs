using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCloseToCamera : MonoBehaviour
{
    Renderer objectRenderer;
    Color color;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        color = objectRenderer.material.color;
    }

    void Update()
    {
        var distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        if (distance < 100)
        {
            var alpha = Mathf.Pow(distance / 100, 2);
            objectRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}