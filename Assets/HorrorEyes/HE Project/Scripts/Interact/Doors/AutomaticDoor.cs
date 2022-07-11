using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    [Header("Door General settings")]
    public Animation m_doorAnimation;
    public DoorTrigger m_triggerA;
    public DoorTrigger m_triggerB;
    public GameObject m_triggersParent;
    public string m_open_A_Name;
    public string m_open_B_Name;
    public string m_close_A_Name;
    public string m_close_B_Name;
    public bool m_locked;
    public bool m_jamned;
    public bool m_needPower; //for lift
    public int m_keyId;
    int m_state = 0; /// 0 - close, 1 - open A, 2 - open B

    [Header("Door Sounds settings")]
    [Tooltip("Door opening sound")]
    public AudioClip m_openSound;
    [Tooltip("Door closing sound")]
    public AudioClip m_closeSound;
    [Tooltip("Door unlocking sound")]
    public AudioClip m_unlockSound;

    private void Awake()
    {
        if(m_locked || m_jamned || m_needPower)
        {
            m_triggersParent.SetActive(false);
        }else
        {
            m_triggersParent.SetActive(true);
        }
    }

    public void Unlock()
    {
      
        if (m_unlockSound)
        {
            AudioSource.PlayClipAtPoint(m_unlockSound, transform.position);
        }
        StartCoroutine(WaitForUnlock());
    }

    public void SetPower()
    {
       m_needPower = false;
       m_triggersParent.SetActive(true);
    }

    public void ActorEnterOnTrigger(int triggerID)
    {
        if (!m_locked)
        {


            if (triggerID == 0)
            {
                if (m_state == 0)
                {
                    AudioSource.PlayClipAtPoint(m_openSound, transform.position);
                    m_doorAnimation.Play(m_open_A_Name);
                    m_state = 1;
                }
            }

            if (triggerID == 1)
            {
                if (m_state == 0)
                {
                    AudioSource.PlayClipAtPoint(m_openSound, transform.position);
                    m_doorAnimation.Play(m_open_B_Name);
                    m_state = 2;
                }
            }
        }
    }

    public void ActorExitTrigger(int triggerID)
    {
        if (!m_locked)
        {


            if (!m_triggerA.m_actorOnTrigger && !m_triggerB.m_actorOnTrigger)
            {
                if (m_state == 1)
                {
                    AudioSource.PlayClipAtPoint(m_closeSound, transform.position);
                    m_doorAnimation.Play(m_close_A_Name);
                    m_state = 0;
                }

                if (m_state == 2)
                {
                    AudioSource.PlayClipAtPoint(m_closeSound, transform.position);
                    m_doorAnimation.Play(m_close_B_Name);
                    m_state = 0;
                }
            }
        }
    }

    private IEnumerator WaitForUnlock()
    {
        yield return new WaitForSeconds(1f);
        m_jamned = false;
        m_locked = false;
        m_triggersParent.SetActive(true);

    }
}
