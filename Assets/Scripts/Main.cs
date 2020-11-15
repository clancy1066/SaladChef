using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    MY_GAME_INPUTS lastRead;
    I_GameCharacter[] allCharacters;

    public GameInputWrapper gameInput;


    // Start is called before the first frame update
    void Start()
    {
        allCharacters = GetComponentsInChildren<I_GameCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameInput != null)
            lastRead = GameInputWrapper.GetLastRead();

        if (allCharacters != null)
            foreach (I_GameCharacter gc in allCharacters)
                gc.Execute(lastRead);

    }
}
