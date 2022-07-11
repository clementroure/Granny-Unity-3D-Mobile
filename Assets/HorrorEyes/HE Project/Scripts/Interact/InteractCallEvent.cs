using UnityEngine;
using UnityEngine.Events;


public class InteractCallEvent : MonoBehaviour {


    public UnityEvent interactEvent;

    public void InteractCall()
    {
        interactEvent.Invoke();
    }
}
