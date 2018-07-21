using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCloseToCamera : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color color;
    private float fadeStartDistace = 40;
    
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        color = objectRenderer.material.color;
    }

    void Update()
    {
        var distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        if (distance < fadeStartDistace)
        {
            var alpha = Mathf.Pow(distance / fadeStartDistace, 2);
            objectRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}