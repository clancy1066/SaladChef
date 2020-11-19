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

    // Special FX
    static public List<Floater> sm_freeFloaters = new List<Floater>();
    static Floater              m_floaterTemplate;

    // Start is called before the first frame update
    void Start()
    {
        allCharacters = GetComponentsInChildren<I_GameCharacter>();

        m_floaterTemplate = GetComponentInChildren<Floater>();

        if (m_floaterTemplate != null)
            m_floaterTemplate.gameObject.SetActive(false);
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

    // **********************************************************
    // Special effects (Floaters only)
    // **********************************************************
    static Floater AllocFloater(string text)
    {
        Floater retVal=null;

        if (m_floaterTemplate == null)
            return retVal;

        if (sm_freeFloaters.Count > 0)
        {
            retVal = sm_freeFloaters[0];
            sm_freeFloaters.RemoveAt(0);
        }
        else
        {
            retVal = Instantiate(m_floaterTemplate);
            retVal.AfterInstanceInit();
        } 

        retVal.SetText(text);
        retVal.gameObject.SetActive(true);

        return retVal;
    }

    static public void FreeFloater(Floater floater)
    {
        if (floater == null)
            return;

        floater.gameObject.SetActive(false);

        if (!sm_freeFloaters.Contains(floater))
            sm_freeFloaters.Add(floater);
    }

    static public void SendFloater(Vector3 position,float duration=2.0f,string text=null)
    {
        Floater floater = AllocFloater(text);

        if (floater != null)
            floater.Go(position, duration);
    }

    // **********************************************************
    // Message Handlers
    // **********************************************************
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
