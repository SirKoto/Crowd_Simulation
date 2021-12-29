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
    private Exercise_3_Manager m_manager = null;
    private List<Vector2> m_goal_queue = new List<Vector2>();
    private int m_goal_idx = 0;

    private Coroutine m_rotate_coroutine = null;
    private Coroutine m_direction_coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<AgentInterface>();
        m_manager = gameObject.GetComponentInParent<Exercise_3_Manager>();
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(gameObject.transform.position, new Vector3(m_goal.x, 0.0f, m_goal.y));

        Gizmos.color = Color.green;
        for(int i = Mathf.Max(m_goal_idx - 1, 0); i < m_goal_queue.Count - 1; ++i)
        {
            Vector3 from = new Vector3(m_goal_queue[i].x, 0.0f, m_goal_queue[i].y);
            Vector3 to = new Vector3(m_goal_queue[i+1].x, 0.0f, m_goal_queue[i+1].y);
            Gizmos.DrawLine(from, to);
        }
    }

    private bool is_goal_close()
    {
        return Vector2.SqrMagnitude((new Vector2(transform.position.x, transform.position.z)) - m_goal) < 0.1f;
    }

    private bool is_correct_direction()
    {
        // Because the path regenerates each time
        // We do not need to check if the direction is correct
        return true;
    }

    public void regenerate_A_star()
    {
        m_init = new Vector2(transform.position.x, transform.position.z);
        Vector2 offset = new Vector2(m_manager.get_domain_half_size(), m_manager.get_domain_half_size());
        Vector2 offset05 = new Vector2(m_manager.get_domain_half_size() - 0.5f, m_manager.get_domain_half_size() - 0.5f);

        m_goal_idx = 0;
        m_goal = m_goal_queue[m_goal_queue.Count - 1];
        m_goal_queue = m_manager.m_grid.get_path_to_bidirectional(m_init + offset, m_goal + offset05);

        for (int i = 0; i < m_goal_queue.Count; ++i)
        {
            m_goal_queue[i] -= offset05;
        }

        if (m_goal_queue.Count > 0)
        {
            m_goal = m_goal_queue[m_goal_idx++];
        } else
        {
            m_direction = Vector2.zero;
            m_goal = m_init;
        }

        if(m_direction_coroutine != null)
        {
            StopCoroutine(m_direction_coroutine);
        }
        m_direction_coroutine = StartCoroutine(smooth_change_direction(0.2f));
    }

    private void setup_goal_and_route()
    {
        m_init = new Vector2(transform.position.x, transform.position.z);

        if (m_goal_queue.Count > m_goal_idx)
        {
            m_goal = m_goal_queue[m_goal_idx++];
        }
        else
        {
            Vector2 offset = new Vector2(m_manager.get_domain_half_size(), m_manager.get_domain_half_size());
            m_goal_idx = 0;
            m_goal = m_manager.get_random_empty_pos_0_size();
            m_goal_queue = m_manager.m_grid.get_path_to_bidirectional(m_init + offset, m_goal);

            offset = new Vector2(m_manager.get_domain_half_size() - 0.5f, m_manager.get_domain_half_size() - 0.5f);
            for (int i = 0; i < m_goal_queue.Count; ++i)
            {
                m_goal_queue[i] -= offset;
            }

            if (m_goal_queue.Count > 0)
            {
                m_goal = m_goal_queue[m_goal_idx++];
            } else
            {
                m_goal = m_init;
            }
        }

        if (m_direction_coroutine != null)
        {
            StopCoroutine(m_direction_coroutine);
        }
        m_direction_coroutine = StartCoroutine(smooth_change_direction(0.2f));
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

    private IEnumerator smooth_change_direction(float duration)
    {
        Vector2 start_dir = m_direction;
        Vector2 end_dir = (m_goal - m_init).normalized;

        if (m_rotate_coroutine != null)
        {
            StopCoroutine(m_rotate_coroutine);
        }
        m_rotate_coroutine = StartCoroutine(rotate_agent(end_dir, 1.0f));

        float t = 0.0f;
        while (t < 1.0f)
        {
            m_direction = Vector2.Lerp(start_dir, end_dir, t);
            t += Time.deltaTime / duration;
            yield return null;
        }

        m_direction = end_dir;
    }
}
