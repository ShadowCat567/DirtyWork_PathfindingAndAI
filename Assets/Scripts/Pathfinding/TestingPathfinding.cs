using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//draws a path between two nodes
public class TestingPathfinding : MonoBehaviour
{
    [SerializeField] PathGraph pathfinding; //reference to pathfinding graph class

    [SerializeField] PathNode start; //reference to starting node
    [SerializeField] PathNode end; //reference to ending node

    List<PathNode> path = new List<PathNode>(); //path between starting and ending node

    bool foundPath = false;

    // Start is called before the first frame update
    void Start()
    {
        //pathfinding = GetComponent<PathGraph>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!foundPath && start.getNeighbors().Count > 0) //if we have found a path between the starting and ending nodes, draw it
        {
            path = pathfinding.findPath(start, end);

            foreach (PathNode loc in path)
            {
                loc.GetComponent<SpriteRenderer>().color = Color.red;
            }

            foundPath = true;
        }
    }
}
