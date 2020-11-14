using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    MY_GAME_INPUTS lastRead;

    public GameInputWrapper gameInput;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (gameInput != null)
            lastRead = GameInputWrapper.GetLastRead();

        if (lastRead.trigger1)
            Debug.Log("Trigger 1");

    }
}
