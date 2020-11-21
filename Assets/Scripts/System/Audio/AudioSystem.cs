using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    AudioSource m_info;
    AudioSource m_pickup;
    AudioSource m_putDown;
    AudioSource m_wrong;
    AudioSource m_success;
    AudioSource m_failure;

    // Specials
    AudioSource m_chopping;

    // Ambient music
    AudioSource m_track1;
    AudioSource m_track2;

    void Start()
    {
        m_info      = FindAndPrepare("Info");
        m_pickup    = FindAndPrepare("Pickup");
        m_putDown   = FindAndPrepare("PutDown");
        m_wrong     = FindAndPrepare("Wrong");
        m_success   = FindAndPrepare("Success");
        m_failure   = FindAndPrepare("Failure");

        // Specials
        m_chopping = FindAndPrepare("Chopping");

        m_track1    = FindAndPrepare("Track1");
        m_track2    = FindAndPrepare("Track2");
    }

    AudioSource FindAndPrepare(string name)
    {
        Transform tmp = transform.Find(name);

        if (tmp != null)
            return tmp.GetComponent<AudioSource>();

        return null;
    }

    void TryPlay(AudioSource audioSource,bool onOrOff=true)
    {
        if (audioSource != null)
        {
            if (onOrOff)
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }

    void TrySto(AudioSource audioSource)
    {
        if (audioSource != null)
            audioSource.Play();
    }

    // Dedicated functions. Wrong but fine for this
    public void Info()
    {
        TryPlay(m_info);
    }

    public void Pickup()
    {
        TryPlay(m_pickup);
    }

    public void PutDown()
    {
        TryPlay(m_putDown);
    }

    public void Wrong()
    {
        TryPlay(m_wrong);
    }

    public void Success()
    {
        TryPlay(m_success);
    }

    public void Fail()
    {
        TryPlay(m_failure);
    }

    public void Chopping(bool onOrOff)
    {
        TryPlay(m_chopping,onOrOff);
    }
}
