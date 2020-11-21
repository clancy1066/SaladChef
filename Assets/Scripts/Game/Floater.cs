using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floater : MonoBehaviour
{
    public float    m_floatTimeMax   = 1;
    public float    m_floatTimer     = 0;

    TextMesh  m_text;
    Renderer  m_renderer;

    bool      m_run = false;

    public AnimationCurve   m_alphaFade;

    private void Start()
    {
        AfterInstanceInit();
    }
        
    public void AfterInstanceInit()
    {
        m_renderer  = GetComponentInChildren<Renderer>();
        m_text      = GetComponentInChildren<TextMesh>();
    }

    public void SetText(string text)
    {
        if (m_text != null)
            m_text.text = text;
    }

    public void Go(Vector3 position,float duration)
    {
       transform.parent = null;

       transform.localPosition = Vector3.zero;
       transform.position = position;

        m_floatTimer = m_floatTimeMax = duration;

        m_run = true;
    }

    public void Go(Transform position, float duration)
    {
        Go(position.position, duration);
        
        m_run = true;
    }

    void DriftAway()
    {
        if (m_renderer!=null)
        {
            // Alpha
            float alpha = m_floatTimer / m_floatTimeMax;

            Color color;
            foreach (Material material in m_renderer.materials)
            {
                color = material.color;
                
                color.a = m_alphaFade.Evaluate(alpha);
                
                material.color = color;
            }
        }

        transform.Translate(Vector3.up * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (m_run)
        {
            if (m_floatTimer < 0.0f)
            {
                m_run = false;
                Main.FreeFloater(this);
                return;
            }
            DriftAway();
            m_floatTimer -= Time.deltaTime;
        }
    }
}
