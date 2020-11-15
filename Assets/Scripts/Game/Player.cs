using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,I_GameCharacter
{
    public void Execute(MY_GAME_INPUTS gi) 
    {
        if (gi.trigger1)
            Debug.Log("Trigger 1 again");
    }
}
