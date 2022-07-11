using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    public UnityEvent m_Event;


    public void MyNewAnimationEvent()
    {
        m_Event.Invoke();
    }
}