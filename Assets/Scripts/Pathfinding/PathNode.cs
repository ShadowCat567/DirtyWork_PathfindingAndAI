using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathNode : MonoBehaviour
{
    private Vector2 location { get; set; }
    [SerializeField] private int cost; //cost to travel through a node
    [SerializeField] private int room; //which room node is located in

    [SerializeField] Tilemap map; //tile map that helps orient each of the nodes
    [SerializeField] PathGraph graph; //pathfinding graph of all nodes

    bool isPopulated = false; //tells whether the neighbors list is populated

    [SerializeField] List<PathNode> neighbors = new List<PathNode>(); //list that stores a node's neighboring nodes

    Vector2[] neighborPos = //array that dictates which nodes qualify as neigbors
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    public Vector2 getLocation() { return location; } //gets the location of the node
    public int getCost() { return cost; } //gets the cost of the node
    public void changeCost(int newCost) { cost = newCost; } //allows for the cost of a node to be changed
    public int getRoom() { return room; } //gets the room the node is located in
    public void changeRoom(int newRoom) { room = newRoom; } //allows for a node's room to be changed
    public List<PathNode> getNeighbors() { return neighbors; } //returns the list of neighbors

    private void Start()
    {
        location = transform.position;
        //populateNeighborhood();
        //map = transform.parent.GetComponent<Tilemap>();
    }

    private void Update()
    {
        //makes sure the graph is finished setting up and the neighbor list is not yet populated
        if (graph.getGraph()[room].Count != 0 && !isPopulated)
        {
            populateNeighborhood();
            isPopulated = true; //makes sure neigborhood population only happens once
        }   
    }

    //populates a node's neigborhood list
    void populateNeighborhood()
    {
        foreach (Vector2 nPos in neighborPos) //iterates through the array of possible positions for a neighbor node
        {
            Vector2 pos = location + nPos;

            Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == pos; };

            PathNode perprectiveNeighbor = graph.getGraph()[room].Find(pred);
            
            if (perprectiveNeighbor != null) //if there is a neighboring node, add it to the neighbor list
            {
                neighbors.Add(perprectiveNeighbor);
            }
        }
    }

}
