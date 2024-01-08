using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPathfinding : MonoBehaviour
{
    PathGraph pathfinding;

    [SerializeField] PathNode start;
    [SerializeField] PathNode end;

    List<PathNode> path = new List<PathNode>();

    bool foundPath = false;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = GetComponent<PathGraph>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!foundPath)
        {
            //path = pathfinding.findPath(start, end);

            foreach (PathNode loc in path)
            {
                loc.GetComponent<SpriteRenderer>().color = Color.red;
                Debug.Log(loc.ToString() + "\n");
            }

            foundPath = true;
        }
    }
}
