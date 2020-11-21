using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSetter : MonoBehaviour
{
    public Color m_color = Color.white;

    private void Start()
    {
        SetDefault();
    }

    void SetDefault()
    {
        if (transform.parent != null)
        {
            Renderer renderer = transform.gameObject.GetComponentInChildren<Renderer>();

            if (renderer==null && transform.parent!=null)
                renderer = transform.parent.gameObject.GetComponentInChildren<Renderer>();

            SetColor(renderer);
        }
    }

    //void Update()
    //{
    //    SetDefault();
    //}
    public void SetColor(Renderer renderer)
    {
        if (renderer != null)
            foreach (Material material in renderer.materials)
                material.color = m_color;

    }
}
