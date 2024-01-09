using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq.Expressions;
using System;

public class LeechManager : MonoBehaviour
{
    int health;
    [SerializeField] int maxHealth = 45;

    bool knockedBack = false;

    SpriteRenderer spriteRend;
    Color baseColor = Color.white;
    Color damagedColor = Color.red;

    [SerializeField] float idleVelo = 0.7f;
    [SerializeField] float velocity = 1.5f;

    bool playerSeen = false;
    bool playerInRoom = false;

    [SerializeField] PathGraph pfGraph;
    PathNode pfCurNode; //in the beginning this is set to the leech's starting node
    [SerializeField] int pfRoom;
    [SerializeField] Tilemap pfMap;
    
    BoxCollider2D lc;
    public Rigidbody2D rb { get; set; }

    [System.NonSerialized] public GameObject player;

    Vector3 forceDir = Vector3.zero;

    public bool collided { get; set; }
    public Vector3 moveDirection { get; set; }

    //getters and setter for the variables needed outside this class
    public int getCurHealth() { return health; }
    public float getVelocity() { return velocity; }
    public float getIdleVelocity() { return idleVelo; }
    public void reverseIdleVelo() { idleVelo = -idleVelo; }
    public bool getIsPlayerSeen() { return playerSeen; }
    public void setIsPlayerSeen(bool hasSeenPlayer) { playerSeen = hasSeenPlayer; }
    public bool getPlayerInRoom() { return playerInRoom; }
    public void setPlayerInRoom(bool inRoom) { playerInRoom = inRoom; }
    public int getRoomNum() { return pfRoom; }
    public PathGraph getGraph() { return pfGraph; }
    public PathNode getCurNode() { return pfCurNode; }
    public void setCurNode(PathNode pn) { pfCurNode = pn; }
    public BoxCollider2D getLeechColl() { return lc; }
    public Transform getPlayerPos() { return player.transform; }
    public GameObject getPlayer() { return player; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        baseColor = spriteRend.color;
        lc = this.gameObject.GetComponent<BoxCollider2D>();
        collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            this.gameObject.SetActive(false);

            if (transform.parent == player.transform)
            {
                transform.parent = null;
            }
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
    }

    public float facingDirection(Vector2 startingPosition, Vector2 endingPosition)
    {
        return (endingPosition - startingPosition).x;
    }

    public void ResetLeech()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        baseColor = Color.white;
        spriteRend.color = baseColor;
        health = maxHealth;
    }

    public PathNode getLeechNode()
    {
        Vector3Int tilePos = pfMap.WorldToCell(transform.position);
        Vector3 nodePos = pfMap.CellToWorld(tilePos);
        Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == new Vector2(nodePos.x + 1f, nodePos.y + 1f); };
        PathNode leechPos = pfGraph.getGraph()[pfRoom].Find(pred);

        //leechPos.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.06f, 0.24f);
        return leechPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Knockback")
        {
            knockedBack = true;
            collided = true;

            Vector3 diff = transform.position - collision.transform.position;
            forceDir = diff;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3) //leech collided with a wall
        {
            collided = true;
        }

        if (collision.gameObject.layer == 7) //leech colliced with another leech
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

    public IEnumerator ChangeColor(Color color)
    {
        spriteRend.color = color;
        yield return new WaitForSeconds(0.4f);
        spriteRend.color = baseColor;
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        knockedBack = false;
    }
}
