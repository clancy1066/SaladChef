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
    
    PlayerModel[]   m_models;
    Animator        m_animator;

    Rigidbody       m_rb;
    public float    m_moveSpeed = 1.0f;
    public float    m_moveBoost = 1.0f;
    public float    m_turnSpeed = 1.0f;

    int             m_score;

    public GameInputWrapper m_gameInput;

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

        m_models[index].gameObject.SetActive(true);
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
            MY_GAME_INPUTS gi = m_gameInput.GetLastRead();

            if (gi.trigger1)
            {
                Debug.Log("Trigger 1");
                m_animator.SetBool("Walk", true);
            }

            switch (m_state)
            {
                case PLAYER_STATE.IDLE:     DoIdle (gi);    break;
                case PLAYER_STATE.WALKING:  DoWalking(gi);  break;
                case PLAYER_STATE.TRANSFER: DoTransfer(gi); break;
                case PLAYER_STATE.CHOPPING: DoChopping(gi); break;
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
        HandleNewState();

        if (IsMoving(gi))
            ChangeState(PLAYER_STATE.WALKING);
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

           m_rb.transform.LookAt(delta);
        }
    }
    void DoTransfer(MY_GAME_INPUTS gi)
    {

    }

    void DoChopping(MY_GAME_INPUTS gi)
    {

    }
}
