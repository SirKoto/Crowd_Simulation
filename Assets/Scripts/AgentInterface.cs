using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentInterface : MonoBehaviour
{
    private Animator m_animat;

    private Rigidbody m_rigidbody;


    private Vector3 m_prev_position;
    private Vector3 m_velocity = Vector3.zero;

    public void look_at(Vector2 dir)
    {
        if (dir != Vector2.zero)
        {
            Vector3 new_dir = new Vector3(dir.x, 0.0f, dir.y);
            gameObject.transform.forward = new_dir.normalized;
        }
    }

    public void set_velocity(Vector2 vel)
    {
        Vector3 vel3 = new Vector3(vel.x, 0.0f, vel.y);

        if (!vel3.Equals(Vector3.zero)) {
            //m_rigidbody.AddForce(vel3, ForceMode.Acceleration);

            m_rigidbody.MovePosition(gameObject.transform.position + Time.fixedDeltaTime * vel3);
        }
    }

    private void set_speed_local_animation(Vector2 vel)
    {
        float v = -vel.x;
        float h = vel.y;
        m_animat.SetFloat("speedX", h);
        m_animat.SetFloat("speedZ", v);
    }

    private void set_speed_world_animation(Vector2 vel)
    {
        Vector3 n_vel = new Vector3(vel.x, 0.0f, vel.y);
        n_vel = gameObject.transform.InverseTransformVector(n_vel);

        this.set_speed_local_animation(new Vector2(n_vel.x, n_vel.z));
    }

    private void set_speed_world_animation(Vector3 vel)
    {
        Vector3 obj_vel = gameObject.transform.InverseTransformVector(vel);

        this.set_speed_local_animation(new Vector2(obj_vel.x, obj_vel.z));
    }


    private void FixedUpdate()
    {
        Vector3 delta_pos = m_rigidbody.transform.position - m_prev_position;
        m_velocity = delta_pos / Time.deltaTime;

        set_speed_world_animation(m_velocity);

        m_prev_position = m_rigidbody.transform.position;

    }

    void Start()
    {
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        if (m_rigidbody == null)
        {
            Debug.LogError("Can't find Rigidbody");
        }

        m_animat = gameObject.GetComponent<Animator>();
        if (m_animat == null)
        {
            Debug.LogError("Can't find Animator");
        }

        m_prev_position = gameObject.transform.position;
    }

    private void LateUpdate()
    {

 
    }

}
