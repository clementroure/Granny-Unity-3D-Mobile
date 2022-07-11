using UnityEngine;

[System.Serializable]
public class EnemyModes
{

    [Header("General Settings")]
    [Tooltip("Enemy Mode Name")]
    public string name;
    public int m_enemyModeId;
    public Enemy[] m_enemys;

}


[System.Serializable]
public class DifficultyModes
{

    [Header("General Settings")]
    [Tooltip("Difficulty Name")]
    public string name;
    public int m_DifficultyModeId;
    public float m_XP_Multiplayer;
    public float m_enemySeeDistance;
    public float m_enemyMoveSpeed;
    public float m_enemyLostTime;
    [Tooltip("Enemy Field of View, for example, -70, 70")]
    public Vector2 m_enemyFOV;

}