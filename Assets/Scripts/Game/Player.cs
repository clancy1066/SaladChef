using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER_ID
{
  ANYONE = 0
, PLAYER1
, PLAYER2
, PLAYER3
, PLAYER4
, MAX
};
// Mainly for controlling the animations
public enum PLAYER_STATE
{
    IDLE = 0
,   WALKING
,   TRANSFER
,   CHOPPING
,   WAITIDLE

}

public struct PLAYER_SCORE
{
    public PLAYER_ID    m_playerID;
    public int          m_value;
    public float        m_timeBonus;

    public void Set(PLAYER_ID newID, int newValue,float timeBonus=0.0f)
    {
        m_playerID      = newID;
        m_value         = newValue;
        m_timeBonus     = timeBonus;
    }
};

public class PLAYER_VITALS
{
    public string   m_name  = "No Name";
    public int      m_score = 0;
    public float    m_timer = 3.0f;

    public PLAYER_VITALS()
    {
        Reset();
}

public void SetName(string name)
    {
        m_name = name;
    }

    public void AddScore(PLAYER_SCORE score)
    {
        m_score += score.m_value;
        m_timer += score.m_timeBonus;

    }

    public void Reset()
    {
        m_name = "No Name";
        m_score = 0;
        m_timer = 300.0f;
    }
};


public class Player : MonoBehaviour,I_GameCharacter
{
    // Animation signals
    const string ANIM_PlayerState = "PlayerState";

    [SerializeField]
    PLAYER_STATE    m_state         = PLAYER_STATE.IDLE;
    bool            m_stateChanged  = false;

    // So objects can tell which player is using them
    public PLAYER_ID m_playerID = PLAYER_ID.ANYONE;

    // For score keeper
    int             sm_playerID;
    PLAYER_VITALS m_playerVitals = new PLAYER_VITALS();


    // 
    PlayerModel     m_model;
    PlayerModel[]   m_models;
    Animator        m_animator;

    Rigidbody       m_rb;
    public float    m_moveSpeed = 1.0f;
    public float    m_moveBoost = 1.0f;
    public float    m_turnSpeed = 1.0f;

    // Hold focus until it is latched by user action
    List<Ingredient>    m_ingredients = new List<Ingredient>();
    Ingredient          m_focusIngredient;
    bool                m_waitingIngredientTransfer;
    const int           cm_MAX_INGREDIENTS = 2;

    //
    ChoppingTable       m_focusChoppingTable;

    // Trashcan
    TrashCan            m_focusTrashCan;

    // Input handing
    public GameInputWrapper m_gameInput;
    MY_GAME_INPUTS          m_lastRead;

    public void Start()
    {
        m_models    = GetComponentsInChildren<PlayerModel>();
        m_animator  = GetComponentInChildren<Animator>();
        m_rb        = GetComponentInChildren<Rigidbody>();

        Choose();

        ChangeState(PLAYER_STATE.IDLE);
    }

    public void Reset()
    {
        m_playerVitals.Reset();
    }

    public void Choose()
    {
        if (m_models == null)
            return;

        foreach (PlayerModel pm in m_models)
            pm.gameObject.SetActive(false);

        int index = Random.Range(0,42)%m_models.Length;

        index = 0;

        m_model = m_models[index];

        m_model.gameObject.SetActive(true);
    }

    bool HandleNewState()
    {
        bool retVal = m_stateChanged;

        m_stateChanged = false;

        return retVal;
    }

   

    void ChangeState(PLAYER_STATE newState)
    {
        m_stateChanged = true;

        m_state = newState;

        if (m_animator != null)
        {
            Debug.Log("Anim State: " + (int)m_state);

            m_animator.SetInteger(ANIM_PlayerState, (int)m_state);
        }
    }

    public bool Execute() 
    {     
        if (m_gameInput != null)
        {
            m_lastRead = m_gameInput.GetLastRead();

            switch (m_state)
            {
                case PLAYER_STATE.IDLE:     DoIdle(m_lastRead);     break;
                case PLAYER_STATE.WALKING:  DoWalking(m_lastRead);  break;
                case PLAYER_STATE.TRANSFER: DoTransfer(m_lastRead); break;
                case PLAYER_STATE.CHOPPING: DoChopping(m_lastRead); break;
            }
        }

        Debug.DrawLine(m_model.transform.position, m_model.transform.position + Vector3.up * 2.0f);

        if (m_playerVitals.m_timer >= 0.0f)
            m_playerVitals.m_timer -= Time.deltaTime;
        else
            m_playerVitals.m_timer = 0.0f;

        return m_playerVitals.m_timer > 0;
;
    }

    void ClearIngredients()
    {
        foreach (Ingredient ingredient in m_ingredients)
            Ingredient.Release(ingredient);

        m_ingredients.Clear();
    }

    void CaptureIngredient()
    {
        Ingredient newIngredient = Ingredient.Grab(m_focusIngredient.m_ingredientType);

        if (newIngredient != null)
        {
            Main.AUDIO_Pickup();

            Main.SendFloater(m_focusIngredient.transform.position, 2.0f, ("You picked up " + newIngredient.m_ingredientType.ToString()));

            Vector3 offset = 0.2f * Ingredient.GetPlacementOffset(m_ingredients.Count);

            newIngredient.transform.localScale      *= 0.75f;
            newIngredient.transform.localPosition   = Vector3.zero;
            newIngredient.transform.position        = m_model.GetHolder().transform.position+offset;
            newIngredient.transform.parent          = m_model.GetHolder().transform;

            if (!m_ingredients.Contains(newIngredient))
                m_ingredients.Add(newIngredient);

            newIngredient.m_grabbedByPlayer = true;

         //   Main.SendFloater(m_model.transform.position, 2.0f, ("You picked up " + newIngredient.m_ingredientType.ToString()));
        }
    }

    void CheckAction(MY_GAME_INPUTS gi)
    {
        if (m_model == null)
            return;

        // Only allow action when stopped
        if (m_state != PLAYER_STATE.IDLE)
            return;

        // No model
        if (m_model == null)
            return;

        // User is calling for an action - Find the context and trigger it
        if (gi.trigger1)
        {
            // Always stop
            FullStop();

            if (m_focusTrashCan!=null)
            {
                Main.AUDIO_PutDown();
                ClearIngredients();
                return;
            }
            // If I have an ingredien, either drop it or put it on the table
            if (m_ingredients.Count>= cm_MAX_INGREDIENTS)
            {
                // If I have an ingredient no table, drop it
                if (m_focusChoppingTable != null)
                {
                    // If I have an ingredient and a table, drop it on the table
                    m_focusChoppingTable.AddIngredients(m_ingredients);

                    ClearIngredients();
                }
                else
                {
                    Main.AUDIO_Wrong();
                    Main.SendFloater(m_model.transform.position + Vector3.up, 3.0f, ("Inventory Full"));
                }
            }
            // Can carry more ingredients
            else
            {
                if (m_focusIngredient != null)
                    ChangeState(PLAYER_STATE.TRANSFER);
                else
                {
                    // Start chopping with a table full of ingredients)
                    if (m_ingredients.Count<1 && m_focusChoppingTable != null && m_focusChoppingTable.HasIngredients())
                        ChangeState(PLAYER_STATE.CHOPPING);
                    else
                    {
                        if (m_focusChoppingTable != null)
                        {
                            m_focusChoppingTable.AddIngredients(m_ingredients);
                            ClearIngredients();
                        }
                    }
                }
            }
        }
    }

    void FullStop()
    {
        if (m_rb != null)
        {
            m_rb.ResetCenterOfMass();
            m_rb.ResetInertiaTensor();

            CorrectAngles();

            //   m_rb.position = m_model.transform.position;
        }
    }

    public void AddScore(PLAYER_SCORE score)
    {
        m_playerVitals.AddScore(score);
    }

    public PLAYER_VITALS GetVitals() { return m_playerVitals; }
        
    // Mini-hack because of animations
    void CorrectAngles()
    {
        Vector3 localEuler = m_model.transform.eulerAngles;

        localEuler.x = 0.0f;
        localEuler.z = 0.0f;

        m_model.transform.eulerAngles = localEuler;
    }

    bool IsMoving(MY_GAME_INPUTS gi)
    {
        float moveMag = Mathf.Abs(gi.moveDirs.sqrMagnitude);

        return moveMag > 0.001f;
    }

    // ***************************************************
    // Player state machine
    // ***************************************************
    void DoIdle(MY_GAME_INPUTS gi)
    {
        if (HandleNewState())
        {
            FullStop();
            return;
        }
        if (IsMoving(gi))
            ChangeState(PLAYER_STATE.WALKING);

        CheckAction(gi);
    }
    
    void DoWalking(MY_GAME_INPUTS gi)
    {
        HandleNewState();

        if (!IsMoving(gi))
            ChangeState(PLAYER_STATE.IDLE);
        else
        {
            // Move code here
            Vector3 delta = Vector3.zero;

            delta.x = gi.moveDirs.x;
            delta.z = gi.moveDirs.y;

            m_rb.AddForce(delta.normalized * m_moveSpeed * m_moveBoost);
    
            delta += m_rb.transform.position;

            delta.y = 0.0f;

            m_model.transform.LookAt(delta);

            CorrectAngles();
        }
    }
    void DoTransfer(MY_GAME_INPUTS gi)
    {
        if (m_focusIngredient==null)
        {
            ChangeState(PLAYER_STATE.IDLE);
            return;
        }
        if (HandleNewState())
        {
            m_waitingIngredientTransfer = false;
        }

        if (!m_waitingIngredientTransfer)
            return;

        CaptureIngredient();

        ChangeState(PLAYER_STATE.IDLE);
    }

    void DoChopping(MY_GAME_INPUTS gi)
    {
        if (HandleNewState())
        {
            if (m_focusChoppingTable != null)
            {
                Transform newPos = m_focusChoppingTable.GetPlayerPos();

                m_model.transform.position = newPos.position;
                m_model.transform.rotation = newPos.rotation;

                Main.AUDIO_Chopping(true);

                FullStop();

                m_focusChoppingTable.Run();
            }

        }

        if (m_focusChoppingTable==null || m_focusChoppingTable.Done())
        {
            Main.AUDIO_Chopping(false);
            ChangeState(PLAYER_STATE.IDLE);
        }
    }

    // Collision callbacks
    
    // Player near a grabbable ingredient
    public void OnFoundIngredient(Ingredient ingredient)
    {
        Debug.Log("OnFoundIngredient");

        m_focusIngredient = ingredient;
    }

    public void OnClearIngredient(Ingredient ingredient)
    {
        Debug.Log("OnClearIngredient");

        m_focusIngredient = null;
    }
    // Player near a chopping table
    public void OnFoundChoppingTable(ChoppingTable choppingTable)
    {
        if (choppingTable.m_playerID == PLAYER_ID.ANYONE || choppingTable.m_playerID == m_playerID)
        {
            Debug.Log("OnFoundTable");
            Main.AUDIO_Info();
            Main.SendFloater(choppingTable.transform.position+Vector3.up, 2.0f, ("Your chopping table"));
            m_focusChoppingTable = choppingTable;
        }
    }

    public void OnClearChoppingTable(ChoppingTable choppingTable)
    {
        Debug.Log("OnClearTable");

        m_focusChoppingTable =null;
    }

    // Player near a chopping table
    public void OnFoundTrashCan(TrashCan trashCan)
    {
        Debug.Log("OnFoundTrashCan");

        m_focusTrashCan = trashCan;
    }

    public void OnClearTrashCan(TrashCan trashCan)
    {
        Debug.Log("OnClearTrashCan");

        m_focusTrashCan = null;
    }

    //  Handling animation event callback
    public void OnIngredientEvent()
    {
        m_waitingIngredientTransfer = true;

        Debug.Log("OnIngredientEvent");
    }

}
