using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class A_Grid
{
    private uint m_resolution;

    public A_Grid(uint resolution)
    {
        m_resolution = resolution;
    }

    public float heuristic(Vector2Int pos, Vector2Int goal)
    {
        return (pos - goal).sqrMagnitude;
    }

    public bool in_bounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < m_resolution && pos.y < m_resolution;
    }

    public List<Vector2> get_path_to(Vector2 start_, Vector2 goal_)
    {

        SimplePriorityQueue<Vector2Int> priorityQueue = new SimplePriorityQueue<Vector2Int>();
        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        List<Vector2> pathAccumulated = new List<Vector2>();

        Vector2Int start = Vector2Int.FloorToInt(start_);
        Vector2Int goal = Vector2Int.FloorToInt(goal_);

        priorityQueue.Enqueue(start, heuristic(start, goal));
        visitedNodes.Add(start);


        uint num_iterations = 0;

        while (priorityQueue.Count > 0)
        {
            if(num_iterations++ > m_resolution * m_resolution)
            {
                Debug.LogError("TOO MANY ITERATIONS: " + num_iterations);
                break;
            }

            Vector2Int pos = priorityQueue.Dequeue();
            pathAccumulated.Add(pos);

            if(pos == goal)
            {
                break;
            }

            Vector2Int new_pos = pos + new Vector2Int(1, 0);
            if (in_bounds(new_pos) && !visitedNodes.Contains(new_pos))
            {
                priorityQueue.Enqueue(new_pos, heuristic(new_pos, goal));
                visitedNodes.Add(new_pos);
            }
            new_pos = pos + new Vector2Int(-1, 0);
            if (in_bounds(new_pos) && !visitedNodes.Contains(new_pos))
            {
                priorityQueue.Enqueue(new_pos, heuristic(new_pos, goal));
                visitedNodes.Add(new_pos);
            }
            new_pos = pos + new Vector2Int(0, -1);
            if (in_bounds(new_pos) && !visitedNodes.Contains(new_pos))
            {
                priorityQueue.Enqueue(new_pos, heuristic(new_pos, goal));
                visitedNodes.Add(new_pos);
            }
            new_pos = pos + new Vector2Int(0, 1);
            if (in_bounds(new_pos) && !visitedNodes.Contains(new_pos))
            {
                priorityQueue.Enqueue(new_pos, heuristic(new_pos, goal));
                visitedNodes.Add(new_pos);
            }

        }


        return pathAccumulated;
    }

}
