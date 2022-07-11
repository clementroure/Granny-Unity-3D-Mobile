using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelSetup
{

    [Header("General Settings")]
    [Tooltip("Level name")]
    public string name;
    [Tooltip("Name of scene (need for loading script)")]
    public string m_sceneName;
    [Tooltip("Level description")]
    [TextArea(5,25)]
    public string m_description;
    public GameObject m_levelShowGameobject;
    public bool m_lockByDefault;
    public int m_lockState;
    public int m_price;

    

}

[System.Serializable]
public class EnemySetup
{

    [Header("General Settings")]
    [Tooltip("Enemy name")]
    public string name;
    [Tooltip("Enemy mode id")]
    public int m_enemyModeId;
    [Tooltip("Enemy description")]
    [TextArea(5, 25)]
    public string m_description;
    public GameObject[] m_enemyShowGameobject;
    public bool m_lockByDefault;
    public int m_lockState;
    public int m_price;

}


[System.Serializable]
public class DifficultSetup
{

    [Header("General Settings")]
    [Tooltip("Dificult name")]
    public string name;
    public int m_difficultId;
    [Tooltip("Enemy description")]
    [TextArea(5, 25)]
    public string m_description;

}

