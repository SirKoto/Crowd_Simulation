using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_AStarAI : MonoBehaviour
{

    private Vector2 m_init = Vector2.zero;
    private Vector2 m_goal = Vector2.zero;
    private Vector2 m_direction = Vector2.zero;
    private float m_max_speed = 3.0f;
    private float m_min_speed = 1.2f;
    private float m_speed = 2.0f;

    private AgentInterface m_agent = null;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<AgentInterface>();
        m_speed = Random.Range(m_min_speed, m_max_speed);
        setup_goal_and_route();
    }

    // Update is called once per frame
    void Update()
    {
        m_agent.set_velocity(m_direction * m_speed);


        if (is_goal_close() || !is_correct_direction())
        {
            setup_goal_and_route();
        }

        // Collisions may budge the agent. Correct
        Vector2 delta = m_goal - m_init;
        m_direction = delta.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(gameObject.transform.position, new Vector3(m_goal.x, 0.0f, m_goal.y));
    }

    private bool is_goal_close()
    {
        return Vector2.SqrMagnitude((new Vector2(transform.position.x, transform.position.z)) - m_goal) < 0.5f;
    }

    private bool is_correct_direction()
    {
        Vector2 dir = m_goal - (new Vector2(transform.position.x, transform.position.z));
        float dot = Vector2.Dot(dir, m_direction);

        return dot >= 0.0f;
    }

    private void setup_goal_and_route()
    {

        float size = gameObject.GetComponentInParent<Exercise_3_Manager>().get_domain_half_size() - 0.5f;

        m_init = new Vector2(transform.position.x, transform.position.z);
        do
        {
            m_goal = new Vector2(Random.Range(-size, size), Random.Range(-size, size));
        } while (is_goal_close());

        Vector2 delta = m_goal - m_init;
        m_direction = delta.normalized;
        StartCoroutine(rotate_agent(m_direction, 2.0f));
    }

    private IEnumerator rotate_agent(Vector2 look_to, float duration)
    {
        Quaternion ini = gameObject.transform.rotation;
        Quaternion end = Quaternion.LookRotation(new Vector3(look_to.x, 0.0f, look_to.y));

        float t = 0.0f;
        while (t < 1.0f)
        {
            Quaternion interp = Quaternion.Slerp(ini, end, t);
            gameObject.transform.rotation = interp;
            t += Time.deltaTime / duration;
            yield return null;
        }

        gameObject.transform.rotation = end;
    }
}
