using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise_4_Manager : MonoBehaviour
{

    [SerializeField]
    private uint m_domain_size = 20;
    [SerializeField]
    private uint m_num_agents = 20;

    [SerializeField]
    private GameObject m_floor = null;

    [SerializeField]
    private GameObject m_gameObject_to_instance = null;
    [SerializeField]
    private GameObject m_wall_gameObject = null;

    [SerializeField]
    private float m_collision_avoidance_radius = 1.0f;
    [SerializeField]
    private float m_collision_avoidance_strength = 0.6f;

    [SerializeField]
    private float m_agent_avoidance_radius = 1.0f;
    [SerializeField]
    private float m_agent_avoidance_pow = 3.0f;
    [SerializeField]
    private float m_agent_avoidance_strength = 0.2f;

    public A_Grid m_grid;

    private List<GameObject> m_walls = new List<GameObject>();
    private List<GameObject> m_agents = new List<GameObject>();

    public float get_domain_half_size()
    {
        return (float)m_domain_size / 2.0f;
    }


    public Vector2 get_random_empty_pos_0_size()
    {
        Vector2Int pos;
        do
        {
            pos = new Vector2Int(Random.Range(0, (int)m_domain_size), Random.Range(0, (int)m_domain_size));
        } while (m_grid.is_obstacle(pos));

        Vector2 finalPos = pos + new Vector2(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));
        return finalPos;
    }

    public Vector2 get_random_empty_pos()
    {
        Vector2Int pos;
        do
        {
            pos = new Vector2Int(Random.Range(0, (int)m_domain_size), Random.Range(0, (int)m_domain_size));
        } while (m_grid.is_obstacle(pos));

        Vector2 finalPos = pos + new Vector2(Random.Range(0.2f, 0.8f) - get_domain_half_size(), Random.Range(0.2f, 0.8f) - get_domain_half_size());

        if (finalPos.x < -m_domain_size || finalPos.x > m_domain_size)
        {
            Debug.LogError("Error: bad position x " + finalPos.x.ToString());
        }

        if (finalPos.y < -m_domain_size || finalPos.y > m_domain_size)
        {
            Debug.LogError("Error: bad position y " + finalPos.y.ToString());
        }

        return finalPos;
    }

    public Vector3 get_collision_avoidance(Vector3 pos)
    {
        Vector3 dir = Vector3.zero;
        float radius = Mathf.Sqrt(2.0f * 0.5f * 0.5f) + m_collision_avoidance_radius;

        float sum_weight = 0.0f;
        foreach (GameObject obj in m_walls)
        {
            Vector3 delta = pos - obj.transform.position;
            float sq_dist_wall = Vector2.SqrMagnitude(new Vector2(delta.x - 0.5f, delta.z - 0.5f));
            if(sq_dist_wall < radius * radius)
            {
                float dist = Mathf.Sqrt(sq_dist_wall);
                float t = 1.0f - (dist - Mathf.Sqrt(2.0f * 0.5f * 0.5f)) / m_collision_avoidance_radius;
                float w = t * t;
                sum_weight += w;
                dir += w * delta.normalized;
            }
        }

        if(sum_weight <= 1e-4f)
        {
            return Vector3.zero;
        }

        return m_collision_avoidance_strength / sum_weight * dir;
    }

    public Vector3 get_agent_avoidance(Vector3 pos)
    {
        Vector3 dir = Vector3.zero;
        float radius = m_agent_avoidance_radius;

        float sum_weight = 0.0f;
        foreach (GameObject obj in m_agents)
        {
            Vector3 delta = pos - obj.transform.position;
            float sq_dist = Vector2.SqrMagnitude(new Vector2(delta.x, delta.z));
            if(sq_dist < 1e-3)
            {
                continue;
            }
            if (sq_dist < radius * radius)
            {
                float dist = Mathf.Sqrt(sq_dist);
                float t = 1.0f - dist / m_agent_avoidance_radius;
                float w = Mathf.Pow(t, m_agent_avoidance_pow);
                sum_weight += w;
                dir += w * delta.normalized;
            }
        }

        if (sum_weight <= 1e-4f)
        {
            return Vector3.zero;
        }

        return m_agent_avoidance_strength / sum_weight * dir;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        float radius = Mathf.Sqrt(2.0f * 0.5f * 0.5f) + m_collision_avoidance_radius;
        foreach (GameObject obj in m_walls)
        {
            Gizmos.DrawWireSphere(obj.transform.position + new Vector3(0.5f, 0.0f, 0.5f), radius);
        }
        Gizmos.color = Color.grey;
        foreach(GameObject obj in m_agents)
        {
            Gizmos.DrawWireSphere(obj.transform.position, m_agent_avoidance_radius);
        }
    }

    private IEnumerator regenerate_A_star()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);

            m_grid.reset_penalizations();

            foreach (GameObject obj in m_agents)
            {
                Vector3 pos3 = obj.transform.position;
                Vector2Int pos2 = Vector2Int.FloorToInt(new Vector2(pos3.x + get_domain_half_size(), pos3.z + get_domain_half_size()));
                m_grid.add_penalization(pos2);
            }

            foreach (GameObject obj in m_agents)
            {
                obj.GetComponent<Bot_ReynoldsAI>().regenerate_A_star();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_floor == null)
        {
            Debug.LogError("Floor not assigned");
        }

        float plane_size = m_floor.GetComponent<MeshCollider>().bounds.size.x;
        float scale = (float)(m_domain_size) / plane_size;
        m_floor.transform.localScale = new Vector3(scale, 1.0f, scale);

        if (m_gameObject_to_instance == null)
        {
            Debug.LogError("Game object to instance not assigned");
        }

        if (m_wall_gameObject == null)
        {
            Debug.LogError("Wall Game Object not assigned");
        }

        m_grid = new A_Grid(m_domain_size);

        // Instantiate walls
        Vector3 offset = new Vector3(get_domain_half_size(), 0.0f, get_domain_half_size());
        Random.InitState(15);
        for (int i = 0; i < m_domain_size; ++i)
        {
            for (int j = 0; j < m_domain_size; ++j)
            {
                if (Random.value < 0.1f)
                {
                    m_grid.add_obstacle(new Vector2Int(i, j));
                    Vector3 pos = new Vector3(i, 0.0f, j) - offset;
                    GameObject obj = GameObject.Instantiate(m_wall_gameObject, pos, Quaternion.identity, gameObject.transform);
                    m_walls.Add(obj);
                }
            }
        }

        // Instantiate agents

        GameObject duplicate = GameObject.Instantiate(m_gameObject_to_instance, Vector3.zero, Quaternion.identity, gameObject.transform);


        duplicate.AddComponent<Bot_ReynoldsAI>();

        for (uint i = 0; i < m_num_agents; ++i)
        {
            Vector2 pos2 = get_random_empty_pos();
            Vector3 pos = new Vector3(pos2.x, 0.0f, pos2.y);
            GameObject obj = GameObject.Instantiate(duplicate, pos, Quaternion.identity, gameObject.transform);
            m_agents.Add(obj);
        }

        GameObject.Destroy(duplicate);

        // Start regeneration courutines
        StartCoroutine(regenerate_A_star());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
