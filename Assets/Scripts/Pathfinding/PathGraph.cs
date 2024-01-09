using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

//Priority queue from here: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp 

public class PathGraph : MonoBehaviour
{
    [SerializeField] List<PathNode>[] graph = new List<PathNode>[15]; //array of lists that stores the pathfinding graph

    public List<PathNode>[] getGraph() { return graph; } //gets the graph

    private void Start()
    {
        convertArray(graph);
    }

    //finds a path between two points and returns that path
    //used this to help with gaining a better understanding of A* and how to implement it: https://www.redblobgames.com/pathfinding/a-star/introduction.html
    public List<PathNode> findPath(PathNode start, PathNode end)
    {
        bool pathError = false;

        SimplePriorityQueue<PathNode, float> frontier = new SimplePriorityQueue<PathNode, float>();

        frontier.Enqueue(start, 0);

        List<PathNode> path = new List<PathNode>();
        path.Add(start);

        //keep track of which node to travel to, key is node we are coming from, value is node we are going to
        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
        cameFrom[start] = start;

        //keep track of which neighboring node costs the least to travel to
        Dictionary<PathNode, float> costSoFar = new Dictionary<PathNode, float>();
        costSoFar.Add(start, 10000);

        while (frontier.Count > 0)
        {
            PathNode curNode = frontier.Dequeue();

            //if we have reached the end node, stop forming the path
            if (curNode.getLocation() == end.getLocation())
            {
                break;
            }

            //go through each of the neighbors of the current node to see which will be most optimal to travel through
            //on the way to the end node
            foreach (PathNode pathnode in curNode.getNeighbors())
            {
                if (!costSoFar.ContainsKey(pathnode)) //makes sure we don't double back
                {
                    //cost is equal to the distance between two nodes plus the cost to traverse the next node
                    float newCost = (pathnode.getLocation() - end.getLocation()).magnitude + pathnode.getCost();

                    //if the new cost is lower than the previous lowest cost, replace the node that yielded the previous
                    //lowest cost with this current node as the next node in the path
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

        //reconstructs the path starting from the starting node
        for (int i = 0; i < cameFrom.Count; i++)
        {
            if (!cameFrom.ContainsKey(key)) //occasionally the above algorthim makes invalid paths
            {
                pathError = true;
                break;
            }
            else if (cameFrom[key] == end) //we have reached the end of the path
            {
                break;
            }
            path.Add(cameFrom[key]); //adds node to the path
            key = cameFrom[key]; //use the most recent node as the key to find the next node in the path
        }

        path.Add(end);

        if (pathError) //if we had an invalid path, return null
        {
            return null;
        }

        return path; //if the path was valid, return it
    }

    //returns a random reachable point within the same room as a starting position
    public PathNode randomReachablePoint(int room)
    {
        int nodeInd;

        nodeInd = Random.Range(0, graph[room].Count - 1);

        return graph[room][nodeInd];
    }

    //CURRENTLY IN PROGRESS
    //returns a circular path when given a radius and a center location, favors nodes that are next to walls
    public PathNode[] circularPath(Vector2 center, float radius, int room)
    {
        //draw paths until it makes something that is unobstructed
        return null;
    }

    //puts all of the nodes into the graph based on which room they are located in
    private void convertArray(List<PathNode>[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++) //initializes all of the lists to store nodes in each room
        {
            nodes[i] = new List<PathNode>();
        }

        //iterates through every node in the scene and puts them into the graph based on which room they are located in
        foreach (PathNode pn in FindObjectsOfType<PathNode>()) 
        {
            int room = pn.getRoom();
            nodes[room].Add(pn);
        }
    }
}
