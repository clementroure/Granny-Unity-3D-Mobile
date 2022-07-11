using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCutscene : MonoBehaviour
{

    public Animation[] m_pictures;
    public string m_flyAnim;
    public string m_idleAnim;
    public float m_flyTime;
    int m_pictureID = 0;
    bool m_playing;

    private void Update()
    {
        if(!m_playing)
        {
            m_playing = true;
            m_pictures[m_pictureID].Play(m_flyAnim);
            StartCoroutine(WaitForPictureFly());
        }
    }

    private IEnumerator WaitForPictureFly()
    {

        yield return new WaitForSeconds(m_flyTime);
        if(m_pictureID == m_pictures.Length - 1)
        {
            m_pictureID = 0;
            m_playing = false;
        }
        else
        {
            m_pictureID += 1;
            StartCoroutine(WaitRandom(Random.Range(1,6)));
        }
        

    }

    private IEnumerator WaitRandom(float time)
    {
        yield return new WaitForSeconds(time);
        m_playing = false;

    }

}
