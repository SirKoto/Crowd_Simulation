using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentInterface : MonoBehaviour
{
    private Animator m_animat;


    public void set_speed_local(Vector2 vel)
    {
        float v = vel.x;
        float h = vel.y;
        m_animat.SetFloat("speedX", v);
        m_animat.SetFloat("speedZ", h);
    }

    public void set_speed_world(Vector2 vel)
    {
        Vector3 n_vel = new Vector3(vel.x, 0.0f, vel.y);
        n_vel = gameObject.transform.InverseTransformVector(n_vel);

        this.set_speed_local(new Vector2(n_vel.x, n_vel.z));
    }

    public void look_at(Vector2 dir)
    {
        Vector3 new_dir = new Vector3(dir.x, 0.0f, dir.y);
        gameObject.transform.forward = new_dir.normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_animat = gameObject.GetComponent<Animator>();
    }

}
