using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveSpearAbility : Ability, ITipCallback
{
    public GameObject spear;
    public SpearTip spearTip;
    public GameObject explosiveShell;

    private Material barrelMaterial;
    private Vector3 initialLocalPosition;

    private Timer timeToExplodeAfterGrab = new Timer();

    private void Awake()
    {
        barrelMaterial = spear.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        spear.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = barrelMaterial;

        initialLocalPosition = spear.transform.localPosition;
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        SetDuration(8.0f);

        RelocateSpear();

        spear.SetActive(true);
        spearTip.enabled = true;
        barrelMaterial.SetColor("_EmissionColor", Color.black);
    }

    protected override void AbilityEnd()
    {
        base.AbilityEnd();
        if(spear.transform.parent == transform)
        {
            HideSpear();
        }
    }

    public void TipCollided(GameObject playerGameObject, GameObject blade)
    {
        spear.transform.parent = playerGameObject.transform;
        spear.transform.position = spear.transform.position + spear.transform.forward * 0.5f;
        spearTip.enabled = false;
        timeToExplodeAfterGrab.Reset(3.0f);
    }

    public void RelocateSpear()
    {
        spear.transform.parent = transform;
        spear.transform.localRotation = Quaternion.identity;
        spear.transform.localPosition = initialLocalPosition;
    }

    public void HideSpear()
    {
        spear.transform.parent = null;
        spear.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (!timeToExplodeAfterGrab.Check())
        {
            float progress = timeToExplodeAfterGrab.GetProgress01();
            progress *= progress;

            Color emission = Color.red * (Mathf.Sin(progress * 100.0f) * 0.5f + 0.5f) * progress * 10.0f;
            barrelMaterial.SetColor("_EmissionColor", emission);            
        }

        if (timeToExplodeAfterGrab.CheckOneTimeEvent())
        {
            HideSpear();

            Vector3 spawnPos = spear.transform.position;
            spawnPos.y = 0.0f;

            Instantiate(explosiveShell, spawnPos, Quaternion.identity);
        }
    }

    public override void ResetAbility()
    {
        base.ResetAbility();
        HideSpear();
    }
}
