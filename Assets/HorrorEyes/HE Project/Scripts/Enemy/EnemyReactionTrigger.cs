using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReactionTrigger : MonoBehaviour
{
    public Animation[] m_reactor;
    public string m_idleAnimationName;
    public string m_reactionAnimationName;
    public AudioClip m_reactionSound;
    public AudioSource m_AS;
    public float m_animationFade;
    bool m_enemyHere;
    int state = 0;

    private void Update()
    {
        if(!m_enemyHere && state == 1)
        {
            state = 0;
            for (int i = 0; i < m_reactor.Length; i++)
            {
                m_reactor[i].CrossFade(m_idleAnimationName, m_animationFade);
            }
            m_AS.Pause();
        }

        if (m_enemyHere && state == 0)
        {
            state = 1;
            for (int i = 0; i < m_reactor.Length; i++)
            {
                m_reactor[i].CrossFade(m_reactionAnimationName, m_animationFade);
            }
            m_AS.clip = m_reactionSound;
            m_AS.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            m_enemyHere = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            m_enemyHere = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            m_enemyHere = false;
        }
    }
}
