using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;

public class HI_SCORE
{
    public string  m_name  = "Dummy";
    public int     m_score = 42;

    public void Save(StreamWriter writer)
    {
        if (writer == null)
            return;

        writer.WriteLine((m_name + " " + m_score.ToString()));
    }
    public void Load(StreamReader reader)
    {
        string record = reader.ReadLine();

        if (record!=null)
        {

            string[] parts = record.Split(' ');

       
            m_name  = parts[0];
            m_score = int.Parse(parts[1]);
        }
    }
};

public class HiScores : MonoBehaviour
{
    const int cm_MAXHighScores = 10;
    const string m_path = "Assets/schighscores.txt";

    Dictionary<int, string> m_currentHiScores = new Dictionary<int, string>();

    public Transform m_container;
    Text[]            m_text;

    ScrollView      m_scrollView;
    RectTransform   m_scrollRect;

    void Start()
    {
        if (m_container != null)
            m_text = GetComponentsInChildren<Text>();
        
        ReadFile();
 ;    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void WriteFile()
    {
        HI_SCORE hiScore = new HI_SCORE();

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(m_path,false);
        hiScore.Save(writer);
        
        writer.Close();
    }
    void ReadFile()
    {
        m_currentHiScores.Clear();

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(m_path);

        while(!reader.EndOfStream)
        {
            HI_SCORE hiScore = new HI_SCORE();

            hiScore.Load(reader);

            m_currentHiScores[hiScore.m_score] = hiScore.m_name;
        }
       
        reader.Close();

        PopulatePanel();
    }

    void PopulatePanel()
    {
        int maxCount = m_currentHiScores.Count;
        int textIndex = 0;

        if (maxCount > cm_MAXHighScores)
            maxCount = cm_MAXHighScores;

        if (maxCount > m_text.Length)
            maxCount = m_text.Length;

        foreach (KeyValuePair<int, string> iter in m_currentHiScores)
        {
            m_text[textIndex++].text = (iter.Value + " " + iter.Key);

            if (textIndex >= maxCount)
                break;
        }
    }
}
