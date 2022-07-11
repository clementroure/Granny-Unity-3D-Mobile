using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;


public class Inventory : MonoBehaviour {

    [Header("Inventory Settings")]
    private GameControll m_gameControll;
    [HideInInspector]
    public ItemsDatabase m_itemsDatabase;
    public Transform m_slotsContent;
    public GameObject m_slotPrefab;
    public List<Slot> m_slots = new List<Slot>();

    [Header("UI Settings")]
    public Sprite m_emptySprite;


    private void Awake()
    {
        m_gameControll = GetComponent<GameControll>();
        m_itemsDatabase = GetComponent<ItemsDatabase>();
       
    }


    public void AddItem (int id, int cnt)
    {
        if (id != 0)
        {

            int same = GetSlotWithSameItem(id);

            if (same != -1)
            {
                m_slots[same].m_itemCount += cnt;
                PrepareSlot(m_slots[same]);

            }
            else
            {
                GameObject slt = Instantiate(m_slotPrefab, m_slotsContent);
                Slot newSlot = slt.GetComponent<Slot>();
                newSlot.m_itemID = id;
                newSlot.m_itemCount = cnt;
                m_slots.Add(newSlot);
                PrepareSlot(newSlot);

                if (id == 0) /// if item id == 0 (eyePills id)
                {
                    m_gameControll.AddEyePills(1);
                }

            }
        }else
        {
            if (id == 0) /// if item id == 0 (eyePills id)
            {
                m_gameControll.AddEyePills(1);
            }
        }
    }



    public void RemoveItem(int itemID, int removeCount)
    {
        int same = GetSlotWithSameItem(itemID);

        if(same != -1)
        {
            m_slots[same].m_itemCount -= removeCount;
            m_gameControll.ShowTip(itemID,4);

            if(m_slots[same].m_itemCount <= 0)
            {
                Destroy(m_slots[same].gameObject);
                m_slots.RemoveAt(same);        
            }else
            {
                PrepareSlot(m_slots[same]);
            }
        }


    }



    private void PrepareSlot(Slot slot)
    {

        if (slot.m_itemID != -1)
        {
            int dbID = m_itemsDatabase.GetItemInDatabaseByID(slot.m_itemID);
            if (dbID != -1)
            {
                slot.m_icon.sprite = m_itemsDatabase.Items[dbID].m_itemIcon;
                slot.m_countText.text = slot.m_itemCount.ToString();

            }
        }else
        {
            slot.m_itemCount = 0;
            slot.m_countText.text = "";
            slot.m_icon.sprite = m_emptySprite;
        }
    }


    int GetFreeSlot()
    {
        for (int i = 0; i < m_slots.Count; i++)
        {
            if (m_slots[i].m_itemID == -1)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetSlotWithSameItem(int id)
    {
        for (int i = 0; i < m_slots.Count; i++)
        {
            if(m_slots[i].m_itemID == id)
            {
                return i;
            }
        }

        return -1;
    }


}
