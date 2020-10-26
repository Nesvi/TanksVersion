using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public TextMeshPro abilityText;

    private Transform cameraTransform;

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

        cameraTransform = Camera.main.transform;

        abilityText.text = "";

        ChooseRandomAbility();
    }

    public void Update()
    {
        if (cooldownBetweenAbilities.CheckOneTimeEvent())
        {
            abilityText.text = abilities[currentAbility].gameObject.name;
        }

        if (cooldownBetweenAbilities.Check())
        {
            switch (playerIndex)
            {
                case 1:
                    if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1))
                    {
                        TriggerAbility();
                    }
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Joystick1Button1))
                    {
                        TriggerAbility();
                    }
                    break;
            }
        }

        cooldownBar.value = cooldownBetweenAbilities.GetProgress01() * 100.0f;

        abilityText.transform.rotation = cameraTransform.rotation;
    }

    public void TriggerAbility()
    {
        abilityText.text = "";
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

    private void OnEnable()
    {
        for(int i = 0; i < abilities.Count; i++)
        {
            abilities[i].ResetAbility();
        }
    }
}
