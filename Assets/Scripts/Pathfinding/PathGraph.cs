using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Priority_Queue;

//Priority queue from here: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp 

public class PathGraph : MonoBehaviour
{/*
    [SerializeField] List<PathNode>[] graph = new List<PathNode>[15];

    public List<PathNode>[] getGraph() { return graph; }

    private void Start()
    {
        convertArray(graph);
    }

    //finds a path between two points and returns that path
    public List<PathNode> findPath(PathNode start, PathNode end)
    {
        bool pathError = false;

        SimplePriorityQueue<PathNode, float> frontier = new SimplePriorityQueue<PathNode, float>();

        frontier.Enqueue(start, 0);

        List<PathNode> path = new List<PathNode>();
        path.Add(start);

        //Debug.Log("Start location: " + start.getLocation() + " End Location: " + end.getLocation());

        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
        cameFrom[start] = start;

        Dictionary<PathNode, float> costSoFar = new Dictionary<PathNode, float>();
        costSoFar.Add(start, 10000);

        while (frontier.Count > 0)
        {
            PathNode curNode = frontier.Dequeue();

            if (curNode.getLocation() == end.getLocation())
            {
                break;
            }

            //Debug.Log("Number of neighbors: " + curNode.getNeighbors().Count);

            foreach (PathNode pathnode in curNode.getNeighbors())
            {
                if (!costSoFar.ContainsKey(pathnode))
                {
                    float newCost = (pathnode.getLocation() - end.getLocation()).magnitude + pathnode.getCost();
                    //Debug.Log("Parent Node: " + curNode.ToString() +  ", Node: " + pathnode.ToString() + ", Cost: " + newCost.ToString());
                    if (newCost < costSoFar[curNode])
                    {
                        costSoFar[curNode] = newCost;
                        float heuristic = (pathnode.getLocation() - end.getLocation()).magnitude + pathnode.getCost();
                        frontier.Enqueue(pathnode, heuristic);
                        cameFrom[curNode] = pathnode;
                        costSoFar[pathnode] = 10000;
                    }
                }
            }
        }

        PathNode key = start;

        //Debug.Log(cameFrom.Count);
        /*
        foreach(KeyValuePair<PathNode, PathNode> keyVal in cameFrom)
        {
            Debug.Log("Came from: " + keyVal.Key.getLocation() + " Going to: " + keyVal.Value.getLocation());
        }
        

        for (int i = 0; i < cameFrom.Count; i++)
        {
            if (!cameFrom.ContainsKey(key))
            {
                pathError = true;
                break;
            }
            else if (cameFrom[key] == end)
            {
                break;
            }
            path.Add(cameFrom[key]);
            key = cameFrom[key];
        }

        path.Add(end);

        if (pathError)
        {
            return null;
        }

        return path;
    }

    //returns a random reachable point within the same room as a starting position
    public PathNode randomReachablePoint(Vector2 start, float distance, int room)
    {
        int nodeInd;

        nodeInd = Random.Range(0, graph[room].Count - 1);

        return graph[room][nodeInd];
    }

    //returns a circular path when given a radius and a center location, favors nodes that are next to walls
    //not sure if this one will stay...
    public PathNode[] circularPath(Vector2 center, float radius, int room)
    {
        //draw paths until it makes something that is unobstructed
        return null;
    }

    private void convertArray(List<PathNode>[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new List<PathNode>();
        }

        foreach (PathNode pn in FindObjectsOfType<PathNode>())
        {
            int room = pn.getRoom();
            nodes[room].Add(pn);
        }
        /*
        for (int i = 0; i < nodes.Length; i++)
        {
            Debug.Log("List " + i + ": " + nodes[i].Count);
        }
        
    }
*/
}
