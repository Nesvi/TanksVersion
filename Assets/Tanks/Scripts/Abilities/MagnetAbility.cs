using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetAbility : Ability
{
    public float range = 50.0f;
    public float force = 10.0f;
    public ParticleSystem FX;

    private void Awake()
    {
        FX.gameObject.SetActive(false);
    }

    public override void ResetAbility()
    {
        base.ResetAbility();
        FX.gameObject.SetActive(false);
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        SetDuration(7.0f);
        FX.gameObject.SetActive(true);
        FX.Play();
    }

    protected override void AbilityEnd()
    {
        base.AbilityEnd();
        FX.Stop();
    }

    private void FixedUpdate()
    {
        if (IsAbilityActive())
        {
            Vector3 myPosition = transform.position;

            for (int i = 0; i < GameManager.instance.m_Tanks.Length; i++)
            {
                GameObject otherTank = GameManager.instance.m_Tanks[i].m_Instance.gameObject;

                if (otherTank != gameObject &&
                    !otherTank.GetComponent<TankHealth>().m_Dead)
                {
                    Rigidbody rb = otherTank.GetComponent<Rigidbody>();

                    Vector3 offset = (otherTank.transform.position - myPosition);
                    Vector3 direction = offset.normalized;
                    float distance = offset.magnitude;

                    float rangeFactor = Mathf.Pow(1.0f - Mathf.Clamp01(distance / range), 1.0f / 3.0f);


                    rb.AddForce(direction * force * rangeFactor, ForceMode.Force);

                }
            }
        }
    }
}
