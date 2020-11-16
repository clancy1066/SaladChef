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

    Ingredient      m_focusIngredient;
    Ingredient      m_ingredient;

    // Input handing
    public GameInputWrapper m_gameInput;
    MY_GAME_INPUTS          m_lastRead;

    public void Start()
    {
        m_models    = GetComponentsInChildren<PlayerModel>();
        m_animator  = GetComponentInChildren<Animator>();
        m_rb        = GetComponentInChildren<Rigidbody>();

        Choose();
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

        if (retVal && m_animator != null)
        {
            Debug.Log("Anim State: " + (int)m_state);

            m_animator.SetInteger("PlayerState", (int)m_state);
        }

        return retVal;
    }

    void ChangeState(PLAYER_STATE newState)
    {
        m_stateChanged = true;

        m_state = newState;
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

       UpdateIngredient();

        Debug.DrawLine(m_model.transform.position, m_model.transform.position + Vector3.up * 2.0f);
    }

    void UpdateIngredient()
    {
        if (m_model == null || m_ingredient == null || m_model.GetHolder()==null)
            return;

        m_ingredient.transform.position = m_model.GetHolder().transform.position;

    }

    void CheckAction(MY_GAME_INPUTS gi)
    {
        if (m_model == null)
            return;

        if (gi.trigger1)
        {
            if (m_ingredient!=null)
            {
                Ingredient.Release(m_ingredient);
                m_ingredient = null;
            }

            if (m_focusIngredient != null)
            {
                m_ingredient = Ingredient.Grab(m_focusIngredient.m_ingredientType);
                m_focusIngredient = null;

                if (m_ingredient != null)
                {
                    m_ingredient.transform.localPosition    = Vector3.zero;
                    m_ingredient.transform.position         = m_model.GetHolder().transform.position;
                    m_ingredient.transform.parent = null; ;
                }
            }
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

            Quaternion lookAt = Quaternion.LookRotation(delta);

            m_rb.transform.rotation = Quaternion.Lerp(m_rb.transform.rotation, lookAt, 0.99f); 
        }

        CheckAction(gi);
    }
    void DoTransfer(MY_GAME_INPUTS gi)
    {

    }

    void DoChopping(MY_GAME_INPUTS gi)
    {

    }

    // Collision callback
    public void OnFoundIngredient(Ingredient ingredient)
    {
        Debug.Log("OnFoundIngredient");

        m_focusIngredient = ingredient;
    }

    public void OnFoundChoppingTable(Ingredient ingredient)
    {
        Debug.Log("OnFoundIngredient");

        m_focusIngredient = ingredient;
    }
}
