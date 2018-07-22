using System;
using UnityEngine;
using _20180713._Scripts;

public class PompeiiFalling : MonoBehaviour
{
    private GameStarter gameStarter;
    private Vector3 start;
    private readonly Vector3 end = new Vector3(0, -5, 0);

    private float secondsToFall;
    private float secondsFalling;

    private void Awake()
    {    
        gameStarter = GameObject.FindWithTag("GameStarter").GetComponent<GameStarter>();
        secondsToFall = gameStarter.GameLengthSeconds;

        start = transform.position;
    }

    private void Update()
    {
        secondsFalling += Time.deltaTime;

        var progress = secondsFalling / secondsToFall;
        var curvedProgress = Math.Max(0, EaseInCirc(0, 1, progress));
        var position = transform.position;
        position.y = start.y + curvedProgress * (end.y - start.y);
        transform.position = position.y < end.y ? position : end;
    }

    private float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    private float EaseInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value - 1)) + start;
    }

    float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }
}