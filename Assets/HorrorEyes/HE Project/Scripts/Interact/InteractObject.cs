using UnityEngine;
using UnityEngine.Events;

public class InteractObject : MonoBehaviour
{
    public UnityEvent Interact_Event;
    public int m_itemToInteractId = -1;
    public bool m_useOneTime;
    public bool m_removeItemAfterUse;
    [HideInInspector]
    public bool m_used;

    public void Interact()
    {
        if (!m_used)
        {
            if (m_useOneTime)
            {
                m_used = true;
            }
            Interact_Event.Invoke();
        }
    }

}
