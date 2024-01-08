using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq.Expressions;
using System;

public class LeechManager : MonoBehaviour
{
    bool inBounds = true;

    int health;
    [SerializeField] int maxHealth = 45;

    bool knockedBack = false;

    SpriteRenderer spriteRend;
    Color baseColor = Color.white;
    Color damagedColor = Color.red;

    //UI for Health
    public Slider Slide;
    public GameObject HealthBarUI;

    [SerializeField] float idleVelo = 0.7f;
    [SerializeField] float velocity = 1.5f;
    [SerializeField] float maxAlertDistance = 3.0f;

    //drain while in attack mode
    [SerializeField] float drainTime = 3.0f;
    public float timeRemaining;

    bool playerSeen = false;
    bool playerInRoom = false;

    public Vector3 offset;
    //distance leech attaches at
    float attachDistance = 1.12f;
    //drain while in attached mode
    [SerializeField] float attachedDrain = 1.5f;

    //amount of damage the player takes per drain
    [SerializeField] int drainDamage = 1;

    //leech pathfinding
    [SerializeField] PathGraph pfGraph;
    PathNode pfCurNode; //in the beginning this is set to the leech's starting node
    [SerializeField] List<PathNode> pfPath = new List<PathNode>();
    [SerializeField] int pfRoom;
    public PathNode pfEndNode { get; set; }
    public int index { get; set; }
    [SerializeField] Tilemap pfMap;

    [SerializeField] float longestPathDistance = 3.5f;
    BoxCollider2D lc;
    public Rigidbody2D rb { get; set; }

    [System.NonSerialized] public GameObject player;

    [SerializeField] Tilemap VisualPuddles;
    [SerializeField] GridLayout tileGrid;
    //[SerializeField] PuddleSystem ps;

    [SerializeField]
    private bool inSoapTile = false;
    private bool inSoapDamage = false;
    public bool getInSoapTile() { return inSoapTile; }

    Vector3 forceDir = Vector3.zero;

    public bool collided { get; set; }
    public Vector3 moveDirection { get; set; }

    //getters and setter for the variables needed outside this class
    public int getCurHealth() { return health; }
    public float getVelocity() { return velocity; }
    public float getIdleVelocity() { return idleVelo; }
    public void reverseIdleVelo() { idleVelo = -idleVelo; }
    public float getMaxAlertDist() { return maxAlertDistance; }
    public float getDrainTime() { return drainTime; }
    public bool getIsPlayerSeen() { return playerSeen; }
    public void setIsPlayerSeen(bool hasSeenPlayer) { playerSeen = hasSeenPlayer; }
    public bool getPlayerInRoom() { return playerInRoom; }
    public void setPlayerInRoom(bool inRoom) { playerInRoom = inRoom; }
    public float getAttachDist() { return attachDistance; }
    public float getAttachDrainTime() { return attachedDrain; }
    public int getDrainDmg() { return drainDamage; }
    public List<PathNode> getPath() { return pfPath; }
    public void setPath(List<PathNode> newPath) { pfPath = newPath; }
    public int getRoomNum() { return pfRoom; }
    public PathGraph getGraph() { return pfGraph; }
    public PathNode getCurNode() { return pfCurNode; }
    public void setCurNode(PathNode pn) { pfCurNode = pn; }
    public float getLongestPathDist() { return longestPathDistance; }
    public BoxCollider2D getLeechColl() { return lc; }
    public Transform getPlayerPos() { return player.transform; }
    public GameObject getPlayer() { return player; }

    // Start is called before the first frame update
    void Start()
    {
        //I don't like using find, try to see if there is something better that can achieve the same result...
        player = GameObject.Find("Player");
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        //sr = GetComponent<SpriteRenderer>();
        baseColor = spriteRend.color;
        lc = this.gameObject.GetComponent<BoxCollider2D>();
        collided = false;

        //Kimari Doing prototype healthbar stuff
        Slide.minValue = 0;
        Slide.maxValue = maxHealth;

        //HealthBarUI.SetActive(false);
        Slide.value = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //CheckTile();

        if (health <= 0)
        {
            this.gameObject.SetActive(false);

            if (transform.parent == player.transform)
            {
                transform.parent = null;
            }
        }

        if (inSoapTile)
        {
            SlowTarget();
            //isSoapy = true;
            if (!inSoapDamage)
            {
                inSoapDamage = true;
                StartCoroutine(ISoapDamageCoolDown());
            }
        }
        else
        {
            ReturnToNormalSpeed();
        }
    }

    private void FixedUpdate()
    {
        if (knockedBack)
        {
            rb.AddForce(forceDir * 3f, ForceMode2D.Impulse);
            StartCoroutine(ResetKnockback());
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        StartCoroutine(ChangeColor(damagedColor));

        //Kimari Heath UI stuff
        HealthBarUI.SetActive(true);
        Slide.value = health;

    }

    public void SlowTarget()
    {
        velocity = 0.5f;
    }

    public void ReturnToNormalSpeed()
    {
        velocity = 1.5f;
    }

    public float facingDirection(Vector2 startingPosition, Vector2 endingPosition)
    {
        return (endingPosition - startingPosition).x;
    }

    public void ResetLeech()
    {
        //isSoapy = false;
        spriteRend = GetComponent<SpriteRenderer>();
        baseColor = Color.white;
        spriteRend.color = baseColor;
        health = maxHealth;
        Slide.value = maxHealth;
        //Debug.Log("Current State: " + curState.ToString());
    }

    public PathNode getLeechNode()
    {
        Vector3Int tilePos = pfMap.WorldToCell(transform.position);
        Vector3 nodePos = pfMap.CellToWorld(tilePos);
        Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == new Vector2(nodePos.x + 1f, nodePos.y + 1f); };
        PathNode leechPos = pfGraph.getGraph()[pfRoom].Find(pred);
        //Debug.Log("Node Position = " + tilePos);

        //leechPos.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.06f, 0.24f);
        return leechPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {/*
        Vector3Int tilePos = tileGrid.WorldToCell(collision.gameObject.transform.position);
        TileAttributes data = ps.GetType(tilePos, "Soap");

        if (!data)
        {
            StartCoroutine(InSideWallCountdown());
        }

        if (data.soap)
        {
            //Debug.Log("Leech Manager : Hit with the floor soap!");
            CheckTile();
        }
        */
        if (collision.gameObject.tag == "Knockback")
        {
            //Debug.Log("I am being knocked back");
            knockedBack = true;
            collided = true;
            //ChangeColor(knockbackkColor);

            Vector3 diff = transform.position - collision.transform.position;
            forceDir = diff;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            collided = true;
        }

        if (collision.gameObject.layer == 7)
        {
            if (collision.gameObject.GetComponent<LeechManager>().moveDirection != moveDirection)
            {
                collided = true;
            }
            else
            {
                collided = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //CheckTile();
    }

    public IEnumerator ChangeColor(Color color)
    {
        spriteRend.color = color;
        yield return new WaitForSeconds(0.4f);
        spriteRend.color = baseColor;
    }

    IEnumerator SoapSlow()
    {
        //Debug.Log("Target is slowed");
        SlowTarget();
        yield return new WaitForSeconds(.5f);
        ReturnToNormalSpeed();
        inSoapTile = false;
    }

    IEnumerator ISoapDamageCoolDown()
    {
        TakeDamage(1);
        yield return new WaitForSeconds(0.5f);
        inSoapDamage = false;
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        knockedBack = false;
    }

    IEnumerator InSideWallCountdown()
    {
        yield return new WaitForSeconds(4.0f);
        health = -1;
        gameObject.SetActive(false);
    }

    /*
    private bool CheckTile()
    {
        Vector3Int tilePos = tileGrid.WorldToCell(this.gameObject.transform.position);

        TileAttributes data = ps.GetType(tilePos, "Soap");

        if (!data)
        {
            StartCoroutine(InSideWallCountdown());
        }

        if (data.soap && inSoapTile == false)
        {
            //Debug.Log("Leech Manager :  Entered into soap!");
            inSoapTile = true;
            return inSoapTile;
        }
        if (data.soap && inSoapTile == true)
        {
            //Debug.Log("Leech Manager :  Still in soap!");
            inSoapTile = true;
            return inSoapTile;
        }
        if (!data.soap && inSoapTile)
        {
            //Debug.Log("Leech Manager :  Left the soap!");
            inSoapTile = false;
            return inSoapTile;
        }

        return false;
    }
    */
}
