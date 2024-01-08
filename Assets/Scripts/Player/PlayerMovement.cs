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
    private Vector2 lastMoveDir;
    public bool shouldReact { get; set; }

    float speed;
    [SerializeField] float maxVelocity = 8f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float deceleration = 15f;
    [SerializeField] float curVelo = 2f;

    float maxVeloStorage;
    float accelStorage;

    public Animator animator;

    [SerializeField]
    bool inPuddleTile = false;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] GameObject streamWeapon;
    [SerializeField] GameObject soapWeapon;
    [SerializeField] GameObject meleeWeapon;
    //[SerializeField] WaterShooting waterGun;
    [SerializeField] CameraMovement cam;

    [SerializeField] private Slider _soapStatus;
    [SerializeField] private float _soapRefillSpeed;
    [SerializeField] private float _refillTime = 1.0f;

    bool forceAdded = false;
    float waterForce = 60f;

    private bool _soapEmpty = false;
    private bool _soapActive = false;

    bool hammerActive = false;
    bool hammerLockout = false; //Kimari
    //bool streamActive = true;

    [SerializeField] Tilemap VisualPuddles;
    [SerializeField] GridLayout tileGrid;
    //[SerializeField] PuddleSystem ps;

    //[SerializeField] DialogueManager dm;

    int curRoom = 0;
    [SerializeField] PathGraph pfGraph;
    [SerializeField] Tilemap pfMap;

    PlayerHealth ph;

    public float playerVelo() { return curVelo; }
    public bool IsHammerActive() { return hammerActive; }
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
    //public bool getAttacking() { return attacking; }

    // Start is called before the first frame update
    void Start()
    {
        shouldReact = true;
        rb = GetComponent<Rigidbody2D>();
        ph = GetComponent<PlayerHealth>();

        speed = curVelo;
        maxVeloStorage = maxVelocity;
        accelStorage = acceleration;

        streamWeapon.SetActive(false);
        soapWeapon.SetActive(false);
    }

    private void Update()
    {
        if (streamWeapon.activeSelf)
        {
            cam.ShakeCamera(0.4f);
        }

        meleeWeapon.SetActive(hammerActive);
        //CheckTile();
        if (inPuddleTile)
        {
            //Debug.Log("Player Movement : player is being slowed");
            LowerSpeed(3);
        }
        else
        {
            ReturnSpeed();
        }

        if (_soapActive && _soapStatus != null)
        {
            _soapStatus.value -= _soapRefillSpeed * Time.deltaTime;
            if (_soapStatus.value == 0)
            {
                _soapEmpty = true;
                soapWeapon.SetActive(false);
            }
        }

        if (_soapEmpty && _soapStatus.value < 100)
        {
            _soapStatus.value += _soapRefillSpeed * Time.deltaTime;
        }

        if (_soapEmpty && (100f - _soapStatus.value < .01f))
        {
            _soapStatus.value = 100;
            _soapEmpty = false;
        }
    }

    //animation stuff will probably be somewhere around here
    private void FixedUpdate()
    {
        if (moveVect != Vector2.zero)
        {
            lastMoveDir = moveVect;
        }
        //set animator values
        animator.SetFloat("Horizontal", moveVect.x);
        animator.SetFloat("Vertical", moveVect.y);
        animator.SetFloat("Speed", moveVect.sqrMagnitude);
        animator.SetFloat("lastMoveX", lastMoveDir.x);
        animator.SetFloat("lastMoveY", lastMoveDir.y);

        if (streamWeapon.activeSelf) //&& !forceAdded)
        {
            //if we want a non-continuous force multiply this by something
            //if we want a continous force, it needs to be relatively weak
            rb.AddForce((Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position) * -1 * waterForce,
                ForceMode2D.Force);
            forceAdded = true;
        }

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
        /*
        if (dm != null)
        {
            if (!dm.dialogueIsPlaying)
            {
                //this is how the player actually moves
                rb.velocity = moveVect * curVelo;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
        */
            rb.velocity = moveVect * curVelo;
        //}
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

    //the hammer attack happens here
    public void OnKnockAttack()
    {
        if (!hammerLockout && shouldReact)
        {
            StartCoroutine(ActivateHammer());
        }
    }

    public void OnStreamAttack()
    {
        if (!soapWeapon.activeSelf && shouldReact)
        {
            //waterGun.Shoot();
            streamWeapon.SetActive(true);
        }
    }

    public void OnCancelStream()
    {
        //soapWeapon.SetActive(false);
        streamWeapon.SetActive(false);
    }

    public void OnSoapAttack()
    {
        if (!streamWeapon.activeSelf && !_soapEmpty && shouldReact)
        {
            //attacking = true;
            //soapWeapon.SetActive(true);
            _soapActive = true;
        }
    }

    public void OnCancelSoap()
    {
        //attacking = false;
        //soapWeapon.SetActive(false);
        _soapActive = false;
    }

    /*
    public void OnExitGame()
    {
        //Debug.Log("I should be ending the game");
        SceneManagement.Instance.EndGame();
    }
    */

    //activates and deactivates the hammer, make sure the animation for the hammer is the same
    //length as this
    IEnumerator ActivateHammer()
    {
        hammerActive = true;
        yield return new WaitForSeconds(0.15f);
        hammerActive = false;

        StartCoroutine(HammerCooldown());
    }
    IEnumerator HammerCooldown() //Kimari
    {
        hammerLockout = true;
        yield return new WaitForSeconds(0.5f);
        hammerLockout = false;
    }

    IEnumerator slowPlayer()
    {
        //Debug.Log("Player Movement : player is being slowed");
        LowerSpeed(3);
        yield return new WaitForSeconds(1.0f);
        ReturnSpeed();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //CheckTile();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //CheckTile();
    }

    /*
    private bool CheckTile()
    {
        Vector3Int tilePos = tileGrid.WorldToCell(this.gameObject.transform.position);

        if (VisualPuddles.GetTile(tilePos) == null)
        {
            StartCoroutine(ph.KillPlayer());
            return false;
        }

        TileAttributes data = ps.GetType(tilePos, "Puddle");

        if (!data)
        {
            StartCoroutine(ph.KillPlayer());
        }

        if (data.puddle && inPuddleTile == false)
        {
            //Debug.Log("Player Movement :  Entered into puddle!");
            inPuddleTile = true;
            return inPuddleTile;
        }
        if (!data.puddle && inPuddleTile)
        {
            //Debug.Log("Player Movement : Exit puddle!");
            inPuddleTile = false;
        }

        return false;
    }
    */
}
