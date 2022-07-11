using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{
    private GameControll m_gameControll;

    private void Awake()
    {
        m_gameControll = FindObjectOfType<GameControll>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(m_gameControll.m_currenPicturestCount == m_gameControll.m_needPicturesCount)
            {
                m_gameControll.GameWin();
            }else
            {
                m_gameControll.ShowTip(-1,3);
            }
        }
    }
}
