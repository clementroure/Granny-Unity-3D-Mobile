using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class ItemDrag : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private GameControll m_gameControll;
    private Interact m_interactScript;
    private Slot m_mySlot;
    private Transform m_parent;
    public Transform m_newParent;

     private void Awake()
    {
        m_parent = transform.parent;
        m_interactScript = FindObjectOfType<Interact>();
        m_mySlot = transform.GetComponentInParent<Slot>();
        m_gameControll = FindObjectOfType<GameControll>();
        m_newParent = m_gameControll.gameControllPanel.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
       

        if (m_gameControll.m_mobileTouchInput)
        {
            transform.parent = m_newParent;
            transform.position = Input.GetTouch(0).position;
            
        }else
        {
            transform.parent = m_newParent;
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_interactScript.DropItemCheck(m_mySlot.m_itemID);
        transform.parent = m_parent;
        transform.localPosition = Vector3.zero;
    }
}
