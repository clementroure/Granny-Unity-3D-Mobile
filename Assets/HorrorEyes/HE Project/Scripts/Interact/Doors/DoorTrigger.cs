using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Tooltip("0 or 1")]
    public int triggerID;
    public AutomaticDoor m_automaticDoor;
    public bool m_actorOnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Enemy")
        {
            m_actorOnTrigger = true;
            m_automaticDoor.ActorEnterOnTrigger(triggerID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            m_actorOnTrigger = false;
            m_automaticDoor.ActorExitTrigger(triggerID);
        }
    }
}
