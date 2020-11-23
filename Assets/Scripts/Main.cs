using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE
{
    INIT
,   START
,   GAME
,   SCORES
};

public class Main : MonoBehaviour
{
    // Not a great practice
    static Main _inst;

    Dictionary<PLAYER_ID,Player>    m_playersMap    = new Dictionary<PLAYER_ID, Player>();
    List<Player>                    m_playersList   = new List<Player>();

    public Transform m_startScreen;
    public Transform m_gameScreen;
    public Transform m_hiScoreScreen;
    HiScores         m_hiScores;

   // Score Keeper
   ScoreKeeper      m_scoreKeeper;

    // State machine management
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

    // Audio
    static AudioSystem          m_audioSystem;

    // Start is called before the first frame update
    void Start()
    {
        _inst = this;

        Player[] allCharacters = GetComponentsInChildren<Player>();

        foreach (Player player in allCharacters)
        {
            m_playersMap[player.m_playerID] = player;
            m_playersList.Add(player);
        }

        if (m_hiScoreScreen != null)
            m_hiScores = m_hiScoreScreen.GetComponentInChildren<HiScores>();

        ChangeState(GAME_STATE.INIT);
    }

    // Update is called once per frame
    // ****************************************************
    // Game floaw state machine
    // ****************************************************
    void Update()
    {
        switch (m_gameState)
        {
            case GAME_STATE.INIT:   UpdateInit();   break;
            case GAME_STATE.START:  UpdateStart();  break;
            case GAME_STATE.GAME:   UpdateGame();   break;
            case GAME_STATE.SCORES: UpdateScores(); break;
        }
    }


    void UpdateInit()
    {
        if (HandleStateChanged())
        {
            // One frame for everyhting to settle
            return;
        }

        foreach (Player player in m_playersList)
        {
            player.gameObject.SetActive(true);
            player.Reset();
        }

        m_floaterTemplate   = GetComponentInChildren<Floater>();
        m_scoreKeeper       = GetComponentInChildren<ScoreKeeper>();
        m_audioSystem       = GetComponentInChildren<AudioSystem>();

        if (m_floaterTemplate != null)
            m_floaterTemplate.gameObject.SetActive(false);

        ActivateLevel(m_startScreen,    false);
        ActivateLevel(m_gameScreen,     false);
        ActivateLevel(m_hiScoreScreen,  false);

        if (m_scoreKeeper != null) m_scoreKeeper.gameObject.SetActive(false);

        ChangeState(GAME_STATE.START);
    }
    void UpdateStart()
    {
        if (HandleStateChanged())
        {
            AUDIO_Track1(true);
           
            m_levelComplete = false;
            ActivateLevel(m_startScreen, true);
            ActivateLevel(m_gameScreen, false);
            ActivateLevel(m_hiScoreScreen, false);

        }

        if (m_levelComplete)
        {
            AUDIO_Track1(false);
            ChangeState(GAME_STATE.GAME);
        }
    }
    void UpdateGame()
    {
        if (HandleStateChanged())
        {
            if (m_scoreKeeper != null) m_scoreKeeper.gameObject.SetActive(true);

            ActivateLevel(m_startScreen, false);
            ActivateLevel(m_hiScoreScreen, false);
            ActivateLevel(m_gameScreen, true);

            SetupPlayers();

            AUDIO_Track2(true);
        }

        UpdateUI();

        // Check for jump to high score screen
        bool playersActive = false;

        // Just execute them
        foreach (Player player in m_playersList)
        {
            bool active = player.Execute();

            if (!active)
                player.gameObject.SetActive(false);


             playersActive |= active;
        }

        if (!playersActive)
            ChangeState(GAME_STATE.SCORES);

    }

    void UpdateScores()
    {
        if (HandleStateChanged())
        {
            if (m_scoreKeeper != null) m_scoreKeeper.gameObject.SetActive(true);

            ActivateLevel(m_startScreen, false);
            ActivateLevel(m_hiScoreScreen, true);
            ActivateLevel(m_gameScreen, false);

            if (m_hiScores != null)
            {
                if (m_isTwoPlayerGame)
                {
                    foreach (Player player in m_playersList)
                        m_hiScores.InsertElement(player.GetVitals());

                }
                else
                {
                    Player player1 = (m_playersMap.ContainsKey(PLAYER_ID.PLAYER1) ? m_playersMap[PLAYER_ID.PLAYER1] : null);

                    if (player1 != null)
                        m_hiScores.InsertElement(player1.GetVitals());
                }

                

                m_hiScores.PopulatePanel();

                m_hiScores.WriteFile();
            }

            
            AUDIO_Track2(false);
            AUDIO_Track1(true);

        }

    }

    void ActivateLevel(Transform screen, bool onOrOff)
    {
        if (screen != null)
            screen.gameObject.SetActive(onOrOff);
    }

    // *************** End game state machine ******************/

    void SetupPlayers()
    {
        if (m_playersMap.ContainsKey(PLAYER_ID.PLAYER2))
        {
            m_playersMap[PLAYER_ID.PLAYER2].gameObject.SetActive(m_isTwoPlayerGame);
        }

    }

    void ChangePlayerName(PLAYER_ID playerID,string newName)
    {
        Player player = (m_playersMap.ContainsKey(playerID) ? m_playersMap[playerID] : null);

        if (player != null)
        {
            PLAYER_VITALS vitals = player.GetVitals();

            vitals.m_name = newName;
        }
    }
    public void Player1NameChanged(string newName)
    {
        ChangePlayerName(PLAYER_ID.PLAYER1, newName);
    }

    public void Player2NameChanged(string newName)
    {
        ChangePlayerName(PLAYER_ID.PLAYER2, newName);
    }

    void UpdateUI()
    {
        if (m_scoreKeeper == null)
            return;

        for (PLAYER_ID iter=PLAYER_ID.PLAYER1;iter<PLAYER_ID.MAX;iter++)
        {
            Player player = (m_playersMap.ContainsKey(iter) ? m_playersMap[iter] : null);

            if (player != null)
                m_scoreKeeper.SetScore(iter, player.GetVitals());
        }
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

    public void OnPlayerScored(PLAYER_SCORE score)
    {

        if (score.m_playerID == PLAYER_ID.ANYONE)
        {
            foreach (Player player in m_playersList)
                player.AddScore(score);
        }
        else
        {
            Player player = (m_playersMap.ContainsKey(score.m_playerID) ? m_playersMap[score.m_playerID] : null);

            if (player != null)
                player.AddScore(score);
        }
    }

    public void OnRetry()
    {
        ChangeState(GAME_STATE.INIT);

    }

    public void OnQuit()
    {
        Application.Quit();

    }


    // ******************************************
    // Audio System Helpers
    // ******************************************
    // Dedicated functions. Wrong but fine for this
    static public void AUDIO_Info()
    {
        if (m_audioSystem != null) m_audioSystem.Info();
    }

    static public void AUDIO_Pickup()
    {
        if (m_audioSystem != null) m_audioSystem.Pickup();
    }

    static public void AUDIO_PutDown()
    {
        if (m_audioSystem != null) m_audioSystem.PutDown();
    }

    static public void AUDIO_Wrong()
    {
        if (m_audioSystem != null) m_audioSystem.Wrong();
    }

    static public void AUDIO_Success()
    {
        if (m_audioSystem != null) m_audioSystem.Success();
    }

    static public void AUDIO_Fail()
    {
        if (m_audioSystem != null) m_audioSystem.Fail();
    }

    static public void AUDIO_Chopping(bool onOrOff)
    {
        if (m_audioSystem != null) m_audioSystem.Chopping(onOrOff);
    }

    static public void AUDIO_Track1(bool onOrOff)
    {
        if (m_audioSystem != null) m_audioSystem.Track1(onOrOff);
    }

    static public void AUDIO_Track2(bool onOrOff)
    {
        if (m_audioSystem != null) m_audioSystem.Track2(onOrOff);
    }
}



