using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicAnimatorControllerScript : MonoBehaviour
{

    private AgentInterface m_interface;

    // Start is called before the first frame update
    void Start()
    {
        m_interface = gameObject.GetComponent<AgentInterface>();
        if(m_interface == null)
        {
            Debug.LogError("Can't find Agent Interface");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float h = -2.0f * Input.GetAxis("Horizontal");
        float v = 2.0f * Input.GetAxis("Vertical");
        m_interface.set_speed_world(new Vector2(v, h));

        //m_interface.look_at(new Vector2(v, h));
    }
}
