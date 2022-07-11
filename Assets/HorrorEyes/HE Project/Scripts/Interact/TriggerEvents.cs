using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour {

    [Tooltip("Event that will happen after a player enter in trigger")]
    public UnityEvent interactEvent;

    [HideInInspector]
    public bool activated;
    [HideInInspector]
    public UnityEvent OnSaveEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !activated)
        {
            interactEvent.Invoke();
            activated = true;
        }
    }

    public void LoadState()
    {
        if(activated)
        {
            OnSaveEvent.Invoke();
        }
    }


}
