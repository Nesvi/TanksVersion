using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMineAbility : Ability
{
    public GameObject Ball;
    public GameObject shellExplosion;
    public Transform spawnTransform;

    private GameObject[] currentSpawned;
    private Rigidbody[] spawnedRigidbody;
    private ShellExplosion explosion;
    private Material ballMaterial;

    private bool pendingImpulse;
    const int numberOfInstances = 8;

    private void Awake()
    {
        currentSpawned = new GameObject[numberOfInstances];
        spawnedRigidbody = new Rigidbody[numberOfInstances];

        for (int i = 0; i < numberOfInstances; i++)
        {
            currentSpawned[i] = Instantiate(Ball, spawnTransform.position, spawnTransform.rotation);
            spawnedRigidbody[i] = currentSpawned[i].GetComponent<Rigidbody>();
            currentSpawned[i].SetActive(false);
            if (ballMaterial == null)
                ballMaterial = currentSpawned[i].GetComponent<MeshRenderer>().material;

            currentSpawned[i].GetComponent<MeshRenderer>().material = ballMaterial;
        }
    }
    protected override void AbilityStart()
    {
        base.AbilityStart();
        SetDuration(3.0f);

        for (int i = 0; i < numberOfInstances; i++)
        {
            currentSpawned[i].SetActive(true);


            Vector3 rotatedVector = new Vector3(
                Mathf.Cos(((Mathf.PI * 2.0f) / (float)numberOfInstances) * (float)i),
                1.0f,
                Mathf.Sin(((Mathf.PI * 2.0f) / (float)numberOfInstances) * (float)i)
                );

            currentSpawned[i].transform.position = transform.position + rotatedVector * 2.0f;
            currentSpawned[i].transform.rotation = spawnTransform.rotation;

            pendingImpulse = true;

            currentSpawned[i].GetComponent<ScaleWithCurve>().StartAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (pendingImpulse)
        {
            for (int i = 0; i < numberOfInstances; i++)
            {
                Vector3 rotatedVector = new Vector3(Mathf.Cos(Mathf.PI * 2.0f / 8.0f * i), 2.0f, Mathf.Sin(Mathf.PI * 2.0f / 8.0f * i));
                rotatedVector.Normalize();

                pendingImpulse = false;
                spawnedRigidbody[i].velocity = Vector3.zero;
                spawnedRigidbody[i].angularVelocity = Vector3.zero;
                spawnedRigidbody[i].AddForce(rotatedVector * 2.0f, ForceMode.Impulse);
            }
        }
    }

    protected override void AbilityEnd()
    {
        base.AbilityEnd();
        for (int i = 0; i < numberOfInstances; i++)
        {
            Vector3 spawnPos = currentSpawned[i].transform.position;
            spawnPos.y = 0.0f;

            Instantiate(shellExplosion, spawnPos, Quaternion.identity);

            currentSpawned[i].GetComponent<ScaleWithCurve>().enabled = false;
            currentSpawned[i].transform.localScale = Vector3.zero;
        }

    }

    protected override void Update()
    {
        base.Update();

        if (ballMaterial)
        {
            float progress = duration.GetProgress01();
            progress *= progress;

            Color emission = Color.red * (Mathf.Sin(progress * 100.0f) * 0.5f + 0.5f);
            ballMaterial.SetColor("_EmissionColor", emission);
        }
    }

}
