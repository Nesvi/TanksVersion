using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpearTip : MonoBehaviour
{
    public GameObject ability;
    public float length = 1.0f;
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, length))
        {
            if (hit.collider.gameObject.name.Contains("CompleteTank"))
            {
                ability.GetComponent<ITipCallback>().TipCollided(hit.collider.gameObject, gameObject);
            }
        }
    }
}
