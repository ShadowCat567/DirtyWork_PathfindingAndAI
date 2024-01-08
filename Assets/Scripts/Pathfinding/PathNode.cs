using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathNode : MonoBehaviour
{
    private Vector2 location { get; set; }
    [SerializeField] private int cost;
    //each node knows which room it belongs to
    [SerializeField] private int room;

    [SerializeField] Tilemap map;
    [SerializeField] PathGraph graph;

    bool isPopulated = false;

    [SerializeField] List<PathNode> neighbors = new List<PathNode>();

    Vector2[] neighborPos =
    {
        Vector2.up * 2,
        Vector2.down * 2,
        Vector2.left * 2,
        Vector2.right * 2
    };

    public Vector2 getLocation() { return location; }
    public int getCost() { return cost; }
    //will need to change the cost if a wall or piece of furniture is destroyed
    public void changeCost(int newCost) { cost = newCost; }
    public int getRoom() { return room; }
    public void changeRoom(int newRoom) { room = newRoom; }
    public List<PathNode> getNeighbors() { return neighbors; }

    private void Start()
    {
        location = transform.position;
        //populateNeighborhood();
        //map = transform.parent.GetComponent<Tilemap>();
    }

    private void Update()
    {  
        if (graph.getGraph()[room].Count != 0 && !isPopulated)
        {
            populateNeighborhood();
            isPopulated = true; //makes sure neigborhood population only happens once
        }   
    }

    void populateNeighborhood()
    {
        foreach (Vector2 nPos in neighborPos)
        {
            Vector2 pos = location + nPos;

            Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == pos; };

            PathNode perprectiveNeighbor = graph.getGraph()[room].Find(pred);
            
            if (perprectiveNeighbor != null)
            {
                neighbors.Add(perprectiveNeighbor);
            }
        }
    }

}
