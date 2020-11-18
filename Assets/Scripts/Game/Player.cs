using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mainly for controlling the animations
public enum PLAYER_STATE
{
    IDLE = 0
,   WALKING
,   TRANSFER
,   CHOPPING

}

public class Player : MonoBehaviour,I_GameCharacter
{
    // Animation signals


    [SerializeField]
    PLAYER_STATE    m_state         = PLAYER_STATE.IDLE;
    bool            m_stateChanged  = false;

    // 
    PlayerModel     m_model;
    PlayerModel[]   m_models;
    Animator        m_animator;

    Rigidbody       m_rb;
    public float    m_moveSpeed = 1.0f;
    public float    m_moveBoost = 1.0f;
    public float    m_turnSpeed = 1.0f;

    int             m_score;

    // Hold focus until it is latched by user action
    List<Ingredient>    m_ingredients = new List<Ingredient>();
    Ingredient          m_focusIngredient;
    bool                m_waitingIngredientTransfer;
    const int           cm_MAX_INGREDIENTS = 2;

    //
    ChoppingTable       m_focusChoppingTable;

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

            m_animator.SetInteger("PlayerState", (int)m_state);
        }
    }


    public void Execute() 
    {     
        if (m_gameInput != null)
        {
            m_lastRead = m_gameInput.GetLastRead();

            switch (m_state)
            {
                case PLAYER_STATE.IDLE:     DoIdle(m_lastRead); ;   break;
                case PLAYER_STATE.WALKING:  DoWalking(m_lastRead);  break;
                case PLAYER_STATE.TRANSFER: DoTransfer(m_lastRead); break;
                case PLAYER_STATE.CHOPPING: DoChopping(m_lastRead); break;
            }
        }

        Debug.DrawLine(m_model.transform.position, m_model.transform.position + Vector3.up * 2.0f);
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
            newIngredient.transform.localScale      *= 0.75f;
            newIngredient.transform.localPosition   = Vector3.zero;
            newIngredient.transform.position        = m_model.GetHolder().transform.position;
            newIngredient.transform.parent          = m_model.GetHolder().transform;

            if (!m_ingredients.Contains(newIngredient))
                m_ingredients.Add(newIngredient);
        }
    }

void CheckAction(MY_GAME_INPUTS gi)
    {
        if (m_model == null)
            return;

        // User is calling for an action - Find the context and trigger it
        if (gi.trigger1)
        {
            // If I have an ingredien, either drop it or put it on the table
            if (m_ingredients.Count>= cm_MAX_INGREDIENTS)
            {
                // If I have an ingredient no table, drop it
                if (m_focusChoppingTable!=null)
                {
                    // If I have an ingredient and a table, drop it on the table
                    m_focusChoppingTable.AddIngredients(m_ingredients);
                }

                // Always after an action 
                ClearIngredients();

            }
            // No ingredient
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
                        m_focusChoppingTable.AddIngredients(m_ingredients);
                        ClearIngredients();
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
        }
    }

    bool IsMoving(MY_GAME_INPUTS gi)
    {
        float moveMag = Mathf.Abs(gi.moveDirs.sqrMagnitude);

        return moveMag > 0.001f;
    }

    void DoIdle(MY_GAME_INPUTS gi)
    {
        if (HandleNewState())
        {
            FullStop();
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
        }

        CheckAction(gi);
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

                FullStop();

                m_focusChoppingTable.Run();
            }

        }

        if (m_focusChoppingTable==null || m_focusChoppingTable.Done())
        {
            ChangeState(PLAYER_STATE.IDLE);
        }
    }

    // Collision callbacks
    public void OnFoundIngredient(Ingredient ingredient)
    {
        Debug.Log("OnFoundIngredient");

        m_focusIngredient = ingredient;
    }

    public void OnFoundChoppingTable(ChoppingTable choppingTable)
    {
        Debug.Log("OnFoundTable");

       m_focusChoppingTable = choppingTable;
    }

    public void OnClearIngredient(Ingredient ingredient)
    {
        Debug.Log("OnClearIngredient");

        m_focusIngredient = null;
    }

    public void OnClearChoppingTable(ChoppingTable choppingTable)
    {
        Debug.Log("OnClearTable");

        m_focusChoppingTable =null;
    }

    public void OnIngredientEvent()
    {
        m_waitingIngredientTransfer = true;

        Debug.Log("OnIngredientEvent");
    }
}
