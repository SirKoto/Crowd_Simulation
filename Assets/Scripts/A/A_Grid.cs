using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class A_Grid
{
    private uint m_resolution;
    private HashSet<Vector2Int> m_occupied_pos;

    public A_Grid(uint resolution)
    {
        m_resolution = resolution;
        m_occupied_pos = new HashSet<Vector2Int>();
    }

    public void add_obstacle(Vector2Int pos)
    {
        m_occupied_pos.Add(pos);
    }

    public bool is_obstacle(Vector2Int pos)
    {
        return m_occupied_pos.Contains(pos);
    }

    public float heuristic(Vector2Int pos, Vector2Int goal)
    {
        return (pos - goal).sqrMagnitude;
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
        public bool is_root;
        public AStarDat(float g, float h, Vector2Int parent)
        {
            this.g = g;
            this.h = h;
            this.parent = parent;
            this.is_root = false;
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
            iniDat.is_root = true;
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
                AStarDat dat = new AStarDat(prev_dat.g + heuristic(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }
            new_pos = pos + new Vector2Int(-1, 0);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }
            new_pos = pos + new Vector2Int(0, -1);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic(pos, new_pos), heuristic(new_pos, goal), pos);

                priorityQueue.Enqueue(new_pos, dat.g + dat.h);
                visitedNodes.Add(new_pos, dat);
            }
            new_pos = pos + new Vector2Int(0, 1);
            if (in_bounds(new_pos) && !is_obstacle(new_pos) && !visitedNodes.ContainsKey(new_pos))
            {
                AStarDat dat = new AStarDat(prev_dat.g + heuristic(pos, new_pos), heuristic(new_pos, goal), pos);

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

}
