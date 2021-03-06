using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicAnimatorControllerScript : MonoBehaviour
{


    [SerializeField]
    private AgentInterface m_interface;

    [SerializeField]
    private float m_scale = 3.0f;
    [SerializeField]
    private bool m_fix_rotation = true;

    // Start is called before the first frame update
    void Start()
    {
        //m_interface = gameObject.GetComponent<AgentInterface>();
        if(m_interface == null)
        {
            Debug.LogError("Can't find Agent Interface");
        }
    }

    void Update()
    {
        float h = m_scale * Input.GetAxis("Horizontal");
        float v = m_scale * Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(h, v);
        m_interface.set_velocity(dir);
        if (!m_fix_rotation && dir.sqrMagnitude > 0.1f)
        {
            m_interface.look_at(dir.normalized);
        }

        //m_interface.look_at(new Vector2(v, h));
    }
}
