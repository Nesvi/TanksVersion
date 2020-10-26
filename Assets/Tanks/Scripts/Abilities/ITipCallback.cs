using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITipCallback 
{
    void TipCollided(GameObject playerGameObject, GameObject blade);
}
