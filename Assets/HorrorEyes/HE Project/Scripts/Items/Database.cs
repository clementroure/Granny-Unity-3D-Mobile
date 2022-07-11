using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Database {

    [Header("General Settings")]
    [Tooltip("Item name")]
    public string name;
    [Tooltip("Item description")]
    [TextArea]
    public string m_description;
    [Tooltip("Item ID")]
    public int id;
    [Tooltip("Item icon sprite")]
    public Sprite m_itemIcon;


}

