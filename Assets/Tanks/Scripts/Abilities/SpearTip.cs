using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpearTip : MonoBehaviour
{
    public ExplosiveSpearAbility ability;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f))
        {
            if (hit.collider.gameObject.name.Contains("CompleteTank"))
            {
                ability.TipCollided(hit.collider.gameObject);
            }
        }
    }
}
