using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbility : Ability
{
    public GameObject Wall;
    public Transform spawnTransform;

    private GameObject currentSpawned;
    private Timer destroySpawnedTimer = new Timer();
    private AudioSource audio;

    private void Awake()
    {
        currentSpawned = Instantiate(Wall, spawnTransform.position, spawnTransform.rotation);
        currentSpawned.SetActive(false);
        audio = GetComponent<AudioSource>();
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        SetDuration(4.0f);

        currentSpawned.SetActive(true);
        currentSpawned.transform.position = spawnTransform.position;
        currentSpawned.transform.rotation = spawnTransform.rotation;

        currentSpawned.GetComponent<ScaleWithCurve>().StartAnimation();
        audio.Play();
    }

    protected override void AbilityEnd()
    {
        base.AbilityEnd();
        currentSpawned.GetComponent<ScaleWithCurve>().StartAnimationInverted();
        destroySpawnedTimer.Reset(0.7f);
    }

    protected override void Update()
    {
        base.Update();

        if (destroySpawnedTimer.CheckOneTimeEvent())
            currentSpawned.SetActive(false);
    }

    public override void ResetAbility()
    {
        base.ResetAbility();
        currentSpawned.SetActive(false);
    }

}
