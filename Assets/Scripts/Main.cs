using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE
{
    INIT
,   START
,   GAME
};

public class Main : MonoBehaviour
{
    MY_GAME_INPUTS lastRead;
    I_GameCharacter[] allCharacters;
    Player thePlayer;

    public Transform m_startScreen;
    public Transform m_gameScreen;

    GAME_STATE m_gameState  = GAME_STATE.INIT;
    bool m_gameStateChanged = false;
    bool m_levelComplete    = false;

    bool m_isTwoPlayerGame  = false;

    public  Transform playerSoloStart;

    public  Transform playerMultStart1;
    public  Transform playerMultStart2;

    // Start is called before the first frame update
    void Start()
    {
        allCharacters = GetComponentsInChildren<I_GameCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_gameState)
        {
            case GAME_STATE.INIT:   UpdateInit();   break;
            case GAME_STATE.START:  UpdateStart();  break;
            case GAME_STATE.GAME:   UpdateGame();   break;
        }
    }

    void UpdateInit()
    {
       ChangeState(GAME_STATE.START);
    }
    void UpdateStart()
    {
        if (HandleStateChanged())
        {
            ActivateLevel(m_startScreen, true);
            ActivateLevel(m_gameScreen, false);
        }

        if (m_levelComplete)
            ChangeState(GAME_STATE.GAME);
    }
    void UpdateGame()
    {
        if (HandleStateChanged())
        {
            ActivateLevel(m_startScreen, false);
            ActivateLevel(m_gameScreen, true);

            SetupPlayers();
        }

        if (allCharacters != null)
        foreach (I_GameCharacter gc in allCharacters)
           gc.Execute();
    }

    void SetupPlayers()
    {

    }

    void ChangeState(GAME_STATE newState)
    {
        m_levelComplete     = false;
        m_gameStateChanged  = true;
        m_gameState         = newState;
    }

    bool HandleStateChanged()
    {
        bool retVal = m_gameStateChanged;

        m_gameStateChanged = false;

        return retVal;
    }

    public void OnLevelComplete()
    {
        m_levelComplete = true;
    }

    public void OnOnePlayerClick()
    {
        m_isTwoPlayerGame = false;
    }

    public void OnTwoPlayerClick()
    {
        m_isTwoPlayerGame = true;
    }

    void ActivateLevel(Transform screen, bool onOrOff)
    {
        if (screen != null)
            screen.gameObject.SetActive(onOrOff);
    }
}
