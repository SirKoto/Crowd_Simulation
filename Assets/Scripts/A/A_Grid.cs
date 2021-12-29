using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class A_Grid
{
    private uint m_resolution;
    private HashSet<Vector2Int> m_occupied_pos;
    private Dictionary<Vector2Int, float> m_penalized_pos;

    public A_Grid(uint resolution)
    {
        m_resolution = resolution;
        m_occupied_pos = new HashSet<Vector2Int>();
        m_penalized_pos = new Dictionary<Vector2Int, float>();
    }

    public void add_obstacle(Vector2Int pos)
    {
        m_occupied_pos.Add(pos);
    }

    public bool is_obstacle(Vector2Int pos)
    {
        return m_occupied_pos.Contains(pos);
    }

    public void reset_penalizations()
    {
        m_penalized_pos.Clear();
    }

    public void add_penalization(Vector2Int pos)
    {
        if (m_penalized_pos.ContainsKey(pos))
        {
            m_penalized_pos[pos] += 1.0f;
        }
        else
        {
            m_penalized_pos.Add(pos, 1.0f);
        }
    }

    private float get_penalization(Vector2Int pos)
    {
        float value = 0.0f;
        if(m_penalized_pos.TryGetValue(pos, out value))
        {
            return value * 8.0f;
        }
        else
        {
            return 0.0f;
        }

    }

    public float heuristic(Vector2Int pos, Vector2Int goal)
    {
        return (pos - goal).sqrMagnitude;
    }

    public float heuristic_g(Vector2Int pos, Vector2Int next_pos)
    {
        return get_penalization(next_pos) + heuristic(pos, next_pos);
    }

    public bool in_bounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < m_resolution && pos.y < m_resolution;
    }

    struct AStarDat
    {
        public float g;
        public float h;
        public Vector2Int parent;
        public AStarDat(float g, float h, Vector2Int parent)
        {
            this.g = g;
            this.h = h;
            this.parent = parent;
        }
    }

    public List<Vector2> get_path_to(Vector2 start_, Vector2 goal_)
    {

        SimplePriorityQueue<Vector2Int> priorityQueue = new SimplePriorityQueue<Vector2Int>();
        Dictionary<Vector2Int, AStarDat> visitedNodes = new Dictionary<Vector2Int, AStarDat>();

        Vector2Int start = Vector2Int.FloorToInt(start_);
        Vector2Int goal = Vector2Int.FloorToInt(goal_);

        {
            AStarDat iniDat = new AStarDat(0.0f, heuristic(start, goal), start);
            priorityQueue.Enqueue(start, iniDat.h);
            visitedNodes.Add(start, iniDat);
        }

        uint num_iterations = 0;

        while (priorityQueue.Count > 0)
        {
            if(num_iterations++ > m_resolution * m_resolution)
            {
                Debug.LogError("TOO MANY ITERATIONS: " + num_iterations);
                break;
            }

            Vector2Int pos = priorityQueue.Dequeue();

            if(pos == goal)
            {
                break;
            }

            AStarDat prev_dat = visitedNodes[pos];

            Vector2Int new_pos = pos + new Vector2Int(1, 0);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }
            new_pos = pos + new Vector2Int(-1, 0);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }
            new_pos = pos + new Vector2Int(0, -1);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }
            new_pos = pos + new Vector2Int(0, 1);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }

        }

        // Retrieve the path
        List<Vector2> pathAccumulated = new List<Vector2>();
        Vector2Int retrieve_pos = goal;
        if(!visitedNodes.ContainsKey(retrieve_pos))
        {
            return pathAccumulated;
        }

        while(retrieve_pos != start)
        {
            AStarDat retrieve_dat = visitedNodes[retrieve_pos];
            pathAccumulated.Add(retrieve_pos);
            retrieve_pos = retrieve_dat.parent;
        }

        pathAccumulated.Reverse();

        return pathAccumulated;
    }

    public List<Vector2> get_path_to_bidirectional(Vector2 start_, Vector2 goal_)
    {

        SimplePriorityQueue<Vector2Int> priorityQueue0 = new SimplePriorityQueue<Vector2Int>();
        SimplePriorityQueue<Vector2Int> priorityQueue1 = new SimplePriorityQueue<Vector2Int>();

        Dictionary<Vector2Int, AStarDat> visitedNodes0 = new Dictionary<Vector2Int, AStarDat>();
        Dictionary<Vector2Int, AStarDat> visitedNodes1 = new Dictionary<Vector2Int, AStarDat>();

        Vector2Int start = Vector2Int.FloorToInt(start_);
        Vector2Int goal = Vector2Int.FloorToInt(goal_);

        {
            AStarDat iniDat = new AStarDat(0.0f, heuristic(start, goal), start);
            priorityQueue0.Enqueue(start, iniDat.h);
            visitedNodes0.Add(start, iniDat);
        }
        {
            AStarDat iniDat = new AStarDat(0.0f, heuristic(goal, start), goal);
            priorityQueue1.Enqueue(goal, iniDat.h);
            visitedNodes1.Add(goal, iniDat);
        }

        uint num_iterations = 0;

        Vector2Int intersection = Vector2Int.zero;
        bool intersectionFound = false;
        while (priorityQueue0.Count > 0 && priorityQueue1.Count > 0)
        {
            if (num_iterations++ > m_resolution * m_resolution)
            {
                Debug.LogError("TOO MANY ITERATIONS: " + num_iterations);
                break;
            }

            // START TO GOAL
            {
                Vector2Int pos = priorityQueue0.Dequeue();

                if (pos == goal || visitedNodes1.ContainsKey(pos))
                {
                    intersection = pos;
                    intersectionFound = true;
                    break;
                }

                AStarDat prev_dat = visitedNodes0[pos];

                Vector2Int new_pos = pos + new Vector2Int(1, 0);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes0.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue0.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes0.Add(new_pos, dat);
                }
                new_pos = pos + new Vector2Int(-1, 0);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes0.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue0.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes0.Add(new_pos, dat);
                }
                new_pos = pos + new Vector2Int(0, -1);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes0.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue0.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes0.Add(new_pos, dat);
                }
                new_pos = pos + new Vector2Int(0, 1);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes0.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue0.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes0.Add(new_pos, dat);
                }
            }

            // GOAL TO START
            {
                Vector2Int pos = priorityQueue1.Dequeue();

                if (pos == start || visitedNodes0.ContainsKey(pos))
                {
                    intersection = pos;
                    intersectionFound = true;
                    break;
                }

                AStarDat prev_dat = visitedNodes1[pos];

                Vector2Int new_pos = pos + new Vector2Int(1, 0);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes1.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue1.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes1.Add(new_pos, dat);
                }
                new_pos = pos + new Vector2Int(-1, 0);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes1.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue1.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes1.Add(new_pos, dat);
                }
                new_pos = pos + new Vector2Int(0, -1);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes1.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue1.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes1.Add(new_pos, dat);
                }
                new_pos = pos + new Vector2Int(0, 1);
                if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes1.ContainsKey(new_pos))
                {
                    AStarDat dat = new AStarDat(prev_dat.g + heuristic_g(pos, new_pos), heuristic(new_pos, goal), pos);

                    priorityQueue1.Enqueue(new_pos, dat.g + dat.h);
                    visitedNodes1.Add(new_pos, dat);
                }
            }

        }

        // Retrieve the path
        List<Vector2> pathAccumulated = new List<Vector2>();
        Vector2Int retrieve_pos = intersection;
        if (!intersectionFound)
        {
            return pathAccumulated;
        }

        while (retrieve_pos != start)
        {
            AStarDat retrieve_dat = visitedNodes0[retrieve_pos];
            pathAccumulated.Add(retrieve_pos);
            retrieve_pos = retrieve_dat.parent;
        }

        pathAccumulated.Reverse();

        retrieve_pos = intersection;
        while (retrieve_pos != goal)
        {
            AStarDat retrieve_dat = visitedNodes1[retrieve_pos];
            pathAccumulated.Add(retrieve_pos);
            retrieve_pos = retrieve_dat.parent;
        }
        pathAccumulated.Add(goal);

        return pathAccumulated;
    }

}
