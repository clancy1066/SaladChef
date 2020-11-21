using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    Text m_name;
    Text m_scoreText;
    Text m_timerText;

    private void Start()
    {
        Transform tmp;

        tmp = transform.Find("Score");

        if (tmp != null) m_scoreText = tmp.gameObject.GetComponent<Text>();

        tmp = transform.Find("Timer");

        if (tmp != null) m_timerText = tmp.gameObject.GetComponent<Text>();

        SetScore(0);
        SetTime(0);
    }

    public void Set(PLAYER_VITALS vitals)
    {
        SetScore(vitals.m_score);
        SetTime(vitals.m_timer);
    }

    public void SetScore(uint score)
    {
        if (m_scoreText != null) m_scoreText.text = score.ToString();
    }

    public void SetTime(float time)
    {
        int minutes = (int)((time + 0.5f) / 60.0f);
        int seconds = (int)time % 60;

        if (m_scoreText != null) m_scoreText.text = (minutes.ToString() +":" + seconds.ToString());
    }
}
    
