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
    Vector2 moveVect;
    public bool shouldReact { get; set; }

    float speed;
    [SerializeField] float maxVelocity = 8f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float deceleration = 15f;
    [SerializeField] float curVelo = 2f;

    float maxVeloStorage;
    float accelStorage;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] CameraMovement cam;

    [SerializeField] Tilemap VisualPuddles;
    [SerializeField] GridLayout tileGrid;

    int curRoom = 0;
    [SerializeField] PathGraph pfGraph;
    [SerializeField] Tilemap pfMap;

    public float playerVelo() { return curVelo; }
    public void setRoomNum(int newRoom) { curRoom = newRoom; }
    public PathNode getPlayerNode()
    {
        //Debug.Log("CurRoom = " + curRoom);
        Vector3Int tilePos = pfMap.WorldToCell(transform.position);
        Vector3 nodePos = pfMap.CellToWorld(tilePos);
        Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == new Vector2(nodePos.x + 1f, nodePos.y + 1f); };
        PathNode playerPos = pfGraph.getGraph()[curRoom].Find(pred);
        //Debug.Log("Tile Position = " + tilePos);
        //Debug.Log("Node Position = " + nodePos);

        //playerPos.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.06f, 0.24f);
        return playerPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        shouldReact = true;
        rb = GetComponent<Rigidbody2D>();

        speed = curVelo;
        maxVeloStorage = maxVelocity;
        accelStorage = acceleration;
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

        rb.velocity = moveVect * curVelo;
    }

    public void LowerSpeed(float newMaxSpeed)
    {
        maxVelocity = newMaxSpeed;
        curVelo = newMaxSpeed;
        acceleration = 1f;
    }

    public void ReturnSpeed()
    {
        maxVelocity = maxVeloStorage;
        acceleration = accelStorage;
    }

    //this is how we get movement input from the player, there is a function called Move() in the input asset
    public void OnMove(InputValue input)
    {
        Vector2 inputVector = input.Get<Vector2>();

        if (shouldReact)
        {
            //x component of movement
            moveVect.x = inputVector.x;
            //y component of movement
            moveVect.y = inputVector.y;
        }
        else
        {
            moveVect = Vector2.zero;
        }
    }
}

