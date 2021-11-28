using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise_2_Manager : MonoBehaviour
{

    [SerializeField]
    private uint m_domain_size = 20;
    [SerializeField]
    private uint m_num_agents = 20;

    [SerializeField]
    private GameObject m_floor = null;

    [SerializeField]
    private GameObject m_gameObject_to_instance = null;


    public float get_domain_half_size()
    {
        return (float)m_domain_size / 2.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(m_floor == null)
        {
            Debug.LogError("Floor not assigned");
        }

        float plane_size = m_floor.GetComponent<MeshCollider>().bounds.size.x;
        float scale = (float)(m_domain_size) / plane_size;
        m_floor.transform.localScale = new Vector3(scale, 1.0f, scale);

        if(m_gameObject_to_instance == null)
        {
            Debug.LogError("Game object to instance not assigned");
        }

        GameObject duplicate = GameObject.Instantiate(m_gameObject_to_instance, Vector3.zero, Quaternion.identity, gameObject.transform);


        duplicate.AddComponent<Bot_SimpleGoalAI>();

        float size = get_domain_half_size() - 0.5f;
        for (uint i = 0; i < m_num_agents; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-size, size), 0.0f, Random.Range(-size, size));
            GameObject.Instantiate(duplicate, pos, Quaternion.identity, gameObject.transform);
        }

        GameObject.Destroy(duplicate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
