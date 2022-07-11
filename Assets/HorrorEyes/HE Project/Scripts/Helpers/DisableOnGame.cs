using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnGame : MonoBehaviour
{
    MeshRenderer m_render;

    private void Awake()
    {
        m_render = GetComponent<MeshRenderer>();
        m_render.enabled = false;
    }
}
