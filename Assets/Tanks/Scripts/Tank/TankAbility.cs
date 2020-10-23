using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankAbility : MonoBehaviour
{

    public Transform abilitiesParent;
    private List<Ability> abilities;
    public Slider cooldownBar;

    private int currentAbility = -1;
    public int playerIndex;
    public Timer cooldownBetweenAbilities;

    private void Start()
    {
        abilities = new List<Ability>();

        for(int i = 0; i < abilitiesParent.childCount; i++)
        {
            if (abilitiesParent.GetChild(i).gameObject.activeSelf)
            {
                abilities.Add(abilitiesParent.GetChild(i).GetComponent<Ability>());
            }
        }

        cooldownBetweenAbilities = new Timer();
        cooldownBetweenAbilities.Reset(8.0f);

        ChooseRandomAbility();
    }

    public void Update()
    {
        if (cooldownBetweenAbilities.Check())
        {
            switch (playerIndex)
            {
                case 1:
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        TriggerAbility();
                    }
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.RightControl))
                    {
                        TriggerAbility();
                    }
                    break;
            }
        }

        cooldownBar.value = cooldownBetweenAbilities.GetProgress01() * 100.0f;
    }

    public void TriggerAbility()
    {
        abilities[currentAbility].Use();
        ChooseRandomAbility();
        cooldownBetweenAbilities.Reset();
    }

    public void ChooseRandomAbility()
    {
        int candidate;
        for (int i = 0; i < 10; i++)
        {
            candidate = Random.Range(0, abilities.Count);
            if (candidate != currentAbility)
            {
                currentAbility = candidate;
                return;
            }
        }
    }
}
