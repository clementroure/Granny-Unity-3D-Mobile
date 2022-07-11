using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PicturePaper : MonoBehaviour
{
    public enum effectType
    {
        Null,
        EnemyCall,
        EnemyAlwaysSeePlayer,
        UnlimitedStamina,
        AddEyePills,
        Random,
        RandomBad
    }

    public effectType m_pictureEffectType;
    private Animation m_paperAnimation;
    public int pictureLayer = 12;
    public string m_idleAnimationName;
    public string m_showAnimationName;
    public string m_readAnimationName;
    public AudioClip m_pickupSound;
    public AudioClip[] m_readSound;
    public Sprite m_pictureIconUI;


    private void Awake()
    {
        m_paperAnimation = GetComponent<Animation>();
    }

    public void ShowPaper()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            child.gameObject.layer = pictureLayer;
        }
        m_paperAnimation.Play(m_showAnimationName);
    }

    public void ReadPaper()
    {
        m_paperAnimation.Play(m_readAnimationName);
        int r = Random.Range(0, m_readSound.Length);
        AudioSource.PlayClipAtPoint(m_readSound[r],transform.position);
    }



}
