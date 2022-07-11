using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyInteractPoint : MonoBehaviour
{

    [HideInInspector]
    public Enemy m_enemyAI;
    public int m_enemyHandItemId = -1;
    public int m_enemyAnimatorActionId;
    public Transform m_enemyStartActionPoint;
    public bool m_usingOnceTime;
    [HideInInspector]
    public bool m_used;
    [HideInInspector]
    public bool m_alreadyUsed;
    public bool m_useIfSearch;
    public float m_waitTimeToEndEvent;
    public float m_waitTimeToMidleEvent;
    public UnityEvent EnemyStartEvent;
    public UnityEvent EnemyMidleEvent;
    public UnityEvent EnemyEndEvent;



    public void EnemyStartAction()
    {
        m_enemyAI.agent.enabled = false;
        m_enemyAI.animator.SetInteger("ActionId",m_enemyAnimatorActionId);
        m_used = true;
        EnemyStartEvent.Invoke();
        m_enemyAI.transform.position = m_enemyStartActionPoint.position;
        m_enemyAI.transform.rotation = m_enemyStartActionPoint.rotation;
        StartCoroutine(WaitForMidleEvent());
        StartCoroutine(WaitForEndEvent());
    }

    private void EnemyMidleAction()
    {
        if (m_enemyHandItemId != -1)
        {
            m_enemyAI.m_enemyHandsObjects[m_enemyHandItemId].SetActive(true);
        }
        EnemyMidleEvent.Invoke();
    }

    private void EnemyEndAction()
    {
        if (m_enemyHandItemId != -1)
        {
            m_enemyAI.m_enemyHandsObjects[m_enemyHandItemId].SetActive(false);
        }
        m_enemyAI.m_interactPoint = null;
        m_enemyAI.agent.enabled = true;
        m_enemyAI.animator.SetInteger("ActionId", 0);
        m_enemyAI.m_enemyUsingPoint = false;
        m_enemyAI.ResetPatrol();
        EnemyEndEvent.Invoke();
        if(!m_usingOnceTime)
        {
            m_used = false;
        }
    }


    public void BreakAction()
    {
        StopAllCoroutines();
        if (m_enemyHandItemId != -1)
        {
            m_enemyAI.m_enemyHandsObjects[m_enemyHandItemId].SetActive(false);
        }
        m_enemyAI.agent.enabled = true;
        m_enemyAI.m_interactPoint = null;
        m_enemyAI.animator.SetInteger("ActionId", 0);
        m_enemyAI.m_enemyUsingPoint = false;
        m_enemyAI.ResetPatrol();
        if (!m_usingOnceTime)
        {
            m_used = false;
        }

    }


    private IEnumerator WaitForMidleEvent()
    {
        yield return new WaitForSeconds(m_waitTimeToMidleEvent);
        EnemyMidleAction();

    }

    private IEnumerator WaitForEndEvent()
    {
        yield return new WaitForSeconds(m_waitTimeToEndEvent);
        EnemyEndAction();

    }
}
