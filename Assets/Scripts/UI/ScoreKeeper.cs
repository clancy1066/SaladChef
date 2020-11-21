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
    }

    public void SetScore(PLAYER_ID playerID,PLAYER_VITALS vitals)
    {
        int index = (int)playerID-1;

        if (index<m_playerScores.Length)
        {
            m_playerScores[index].Set(vitals);
        }
    }
}
