using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    Inventory m_inventory;
    public Image m_icon;
    public Text m_countText;
    public int m_itemID = -1;
    public int m_itemCount;


    private void Awake()
    {
        m_inventory = FindObjectOfType<Inventory>();
    }


}
