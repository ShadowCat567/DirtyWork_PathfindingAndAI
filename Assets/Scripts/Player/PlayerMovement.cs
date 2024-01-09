using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveVect; //vector that stores direction player is moving

    float speed; //player's current speed
    [SerializeField] float maxVelocity = 8f; //max speed player can move at
    [SerializeField] float acceleration = 2f; //how fast player accelerates
    [SerializeField] float deceleration = 15f; //how fast player decelerates
    [SerializeField] float curVelo = 2f; //min speed player can move at

    [SerializeField] Rigidbody2D rb;

    int curRoom = 0;
    [SerializeField] PathGraph pfGraph; //reference to pathfinding graph
    [SerializeField] Tilemap pfMap; //reference to pathfinding tileMap the nodes are aligned to

    //gets the player's current velocity
    public float playerVelo() { return curVelo; }
    //gets the room the player is currently in
    public void setRoomNum(int newRoom) { curRoom = newRoom; }
    public PathNode getPlayerNode() //gets the node the player is currently sitting on
    {
        Vector3Int tilePos = pfMap.WorldToCell(transform.position);
        Vector3 nodePos = pfMap.CellToWorld(tilePos);
        Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == new Vector2(nodePos.x + 0.5f, nodePos.y + 0.5f); };
        PathNode playerPos = pfGraph.getGraph()[curRoom].Find(pred);

        //playerPos.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.06f, 0.24f);
        return playerPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        speed = curVelo;
    }

    private void FixedUpdate()
    {
        //while the player is moving, acceleration is applied
        if (Mathf.Abs(moveVect.y) > 0 || Mathf.Abs(moveVect.x) > 0)
        {
            curVelo += acceleration * Time.fixedDeltaTime;
        }
        //when the players stops moving, they decelerate
        else
        {
            curVelo -= deceleration * Time.fixedDeltaTime;

        }
        //keeps their velocity between 2.0 and 8.0
        curVelo = Mathf.Clamp(curVelo, speed, maxVelocity);

        rb.velocity = moveVect * curVelo; //actually moves the player
    }

    //this is how we get movement input for the player, there is a function called Move() in the input asset
    public void OnMove(InputValue input)
    {
        Vector2 inputVector = input.Get<Vector2>();

        //x component of movement
        moveVect.x = inputVector.x;
        //y component of movement
        moveVect.y = inputVector.y;
    }
}

