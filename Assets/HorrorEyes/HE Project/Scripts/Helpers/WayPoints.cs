using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public float m_sphereDrawRadius;
    public Color32 m_color;


    void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = m_color;
            Gizmos.DrawSphere(transform.GetChild(i).position, m_sphereDrawRadius);
        }
    }
}
