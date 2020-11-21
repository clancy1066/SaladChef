using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    PlayerScoreUI[] m_playerScores = null;

    private void Start()
    {
        m_playerScores = GetComponentsInChildren<PlayerScoreUI>();

        if (m_playerScores!=null)
        {
            SetName(PLAYER_ID.PLAYER1, "Player 1");
            SetName(PLAYER_ID.PLAYER1, "Player 2");
        }
    }

    public void SetName(PLAYER_ID playerID,string newName)
    {
        if (m_playerScores == null)
            return;

        int index = (int)playerID - 1;

        if (index < m_playerScores.Length)
        {
            m_playerScores[index].SetName(newName);
        }
    }
    public void SetScore(PLAYER_ID playerID,PLAYER_VITALS vitals)
    {
        if (m_playerScores == null)
            return;

        int index = (int)playerID-1;

        if (index<m_playerScores.Length)
        {
            m_playerScores[index].Set(vitals);
        }
    }
}
