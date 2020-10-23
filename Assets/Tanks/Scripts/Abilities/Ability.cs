using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected Timer duration = new Timer();
    protected bool checkDuration = false;

    protected virtual void Update()
    {
        if (checkDuration && duration.CheckOneTimeEvent())
            AbilityEnd();
    }

    protected void Use()
    {
        AbilityStart();
    }

    protected virtual void AbilityStart()
    {

    }

    protected virtual void AbilityEnd()
    {

    }

    protected void SetDuration(float time)
    {
        duration.Reset(time);
        checkDuration = true;
    }
}
