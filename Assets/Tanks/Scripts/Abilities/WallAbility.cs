using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbility : Ability
{
    public GameObject Wall;
    public Transform spawnTransform;

    private GameObject currentSpawned;
    private Timer destroySpawnedTimer = new Timer();

    private void Awake()
    {
        currentSpawned = Instantiate(Wall, spawnTransform.position, spawnTransform.rotation);
        currentSpawned.SetActive(false);
    }
    protected override void AbilityStart()
    {
        base.AbilityStart();
        SetDuration(4.0f);

        currentSpawned.SetActive(true);
        currentSpawned.transform.position = spawnTransform.position;
        currentSpawned.transform.rotation = spawnTransform.rotation;

        currentSpawned.GetComponent<ScaleYWithCurve>().StartAnimation();
    }

    protected override void AbilityEnd()
    {
        base.AbilityEnd();
        currentSpawned.GetComponent<ScaleYWithCurve>().StartAnimationInverted();
        destroySpawnedTimer.Reset(0.7f);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Use();
        }

        if (destroySpawnedTimer.CheckOneTimeEvent())
            currentSpawned.SetActive(false);
    }

}
