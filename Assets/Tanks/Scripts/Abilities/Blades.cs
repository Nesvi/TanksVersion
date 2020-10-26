using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blades : Ability, ITipCallback
{
    public GameObject explosionEffect;

    private struct PendingImpulse
    {
        public Rigidbody body;
        public Vector3 impulse;
    }

    private List<Transform> blades;
    private List<PendingImpulse> pendingImpulses;
    private AudioSource audio;

    private void Awake()
    {
        pendingImpulses = new List<PendingImpulse>();

        blades = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            blades.Add(transform.GetChild(i));
        }

        audio = GetComponent<AudioSource>();

        ResetAbility();
    }

    public override void ResetAbility()
    {
        for (int i = 0; i < blades.Count; i++)
        {
            blades[i].GetComponent<ScaleWithCurve>().enabled = false;
            blades[i].localScale = Vector3.zero;
        }
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        SetDuration(7.0f);

        for (int i = 0; i < blades.Count; i++)
        {
            blades[i].GetComponent<ScaleWithCurve>().StartAnimation();
            blades[i].GetComponent<SpearTip>().enabled = true;
        }
    }

    protected override void AbilityEnd()
    {
        base.AbilityEnd();

        for (int i = 0; i < blades.Count; i++)
        {
            SpearTip tip = blades[i].GetComponent<SpearTip>();
            if (tip.enabled)
            {
                blades[i].GetComponent<ScaleWithCurve>().StartAnimationInverted();
                tip.enabled = false;
            }
        }
    }

    public void TipCollided(GameObject playerGameObject, GameObject blade)
    {
        blade.GetComponent<ScaleWithCurve>().StartAnimationInverted();
        blade.GetComponent<SpearTip>().enabled = false;

        pendingImpulses.Add(
            new PendingImpulse
            {
                body = playerGameObject.GetComponent<Rigidbody>(),
                impulse = (playerGameObject.transform.position - transform.position).normalized * 20.0f
            }
            );

        GameObject fx = Instantiate(explosionEffect, playerGameObject.transform.position, Quaternion.identity);
        fx.GetComponent<ParticleSystem>().Play();
        Destroy(fx, 2.0f);

        playerGameObject.GetComponent<TankHealth>().TakeDamage(50.0f);
        audio.Play();

    }

    private void FixedUpdate()
    {
        PendingImpulse pendingElement;

        for (int i = pendingImpulses.Count - 1; i >= 0; i--)
        {

            pendingElement = pendingImpulses[i];
            pendingElement.body.AddForce(pendingElement.impulse, ForceMode.Impulse);

            pendingImpulses.RemoveAt(i);
        }
    }
}
