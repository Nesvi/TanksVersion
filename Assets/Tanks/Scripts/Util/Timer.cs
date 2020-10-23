using System;
using UnityEngine;

public class Timer
{

    float startTime;
    float waitTime;
    bool on = false;

    public Timer()
    {
        waitTime = 0.01f;
    }

    public Timer(float _waitTime)
    {
        waitTime = _waitTime;
    }

    public void Reset()
    {
        Reset(waitTime);
    }

    public void Reset(float _waitTime)
    {
        on = true;
        waitTime = _waitTime;
        startTime = Time.timeSinceLevelLoad;
    }

    public bool CheckOneTimeEvent()
    {
        if (on)
        {
            if ((Time.timeSinceLevelLoad - startTime) >= waitTime)
            {
                on = false;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    public bool Check()
    {
        return (Time.timeSinceLevelLoad - startTime) >= waitTime;
    }

    public float GetRemainingTimeInSeconds()
    {
        return waitTime - (Time.timeSinceLevelLoad - startTime);
    }

    public float GetSeconds()
    {
        float remaining = GetRemainingTimeInSeconds()/60;
        return (remaining - Mathf.Floor(remaining)) * 60.0f;
    }
    public float GetMinutes()
    {
        float remaining = GetRemainingTimeInSeconds()/60;
        return Mathf.Floor(remaining);
    }

    public float GetProgress01()
    {
        return 1.0f-Mathf.Clamp(GetRemainingTimeInSeconds() / waitTime,0.0f,1.0f);
    }

    public float GetRemaining01()
    {
        return Mathf.Clamp(GetRemainingTimeInSeconds() / waitTime, 0.0f, 1.0f);
    }

    internal void ForceRemainingTime(float remainingTime)
    {
        startTime = Time.timeSinceLevelLoad - (waitTime - remainingTime);
    }    

    public void ForceEnd()
    {
        ForceRemainingTime(0);
        on = false;
    }
}
