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

    //variables for pathfinding
    [SerializeField] PathGraph pfGraph; //reference to pathfinding graph
    PathNode pfCurNode; //at the start of the game this is set to the leech's starting node
    [SerializeField] int pfRoom; //which room leech is in
    [SerializeField] Tilemap pfMap; //tile map to reference node position
    
    BoxCollider2D lc;
    public Rigidbody2D rb { get; set; }

    [System.NonSerialized] public GameObject player;

    Vector3 forceDir = Vector3.zero; //direction of knockback force

    public bool collided { get; set; } //tells Leech's behavior tree if it collided with something of note
    public Vector3 moveDirection { get; set; } //direction leech is moving in

    //getters and setter for the variables needed outside this class
    public int getCurHealth() { return health; }
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
        if (health <= 0) //if leech's health is >= 0, deactivate the leech
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
        if (knockedBack) //if leech is being knocked back by the player, add a force to it
        {
            rb.AddForce(forceDir * 3f, ForceMode2D.Impulse);
            StartCoroutine(ResetKnockback());
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage) //leech takes damage equal to amount specified by damage parameter
    {
        health -= damage;

        StartCoroutine(ChangeColor(damagedColor)); //leech temporarily changes color to indicate that it took damage
    }

    public void ResetLeech() //after leech dies, reset it so it can be used again by a spawner
    {
        spriteRend = GetComponent<SpriteRenderer>();
        baseColor = Color.white;
        spriteRend.color = baseColor;
        health = maxHealth;
    }

    public PathNode getLeechNode() //return the path node the leech is currently on top of
    {
        Vector3Int tilePos = pfMap.WorldToCell(transform.position);
        Vector3 nodePos = pfMap.CellToWorld(tilePos);
        Predicate<PathNode> pred = (PathNode pn) => { return pn.getLocation() == new Vector2(nodePos.x + 0.5f, nodePos.y + 0.5f); };
        PathNode leechPos = pfGraph.getGraph()[pfRoom].Find(pred);

        //leechPos.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.06f, 0.24f);
        return leechPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Knockback") //if leech was hit by an object held by the player, knock it away from the player
        {
            knockedBack = true;
            collided = true;

            Vector3 diff = transform.position - collision.transform.position; //directon leech should be knocked back in
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

    public IEnumerator ChangeColor(Color color) //cause the leech to temporarily change color
    {
        spriteRend.color = color;
        yield return new WaitForSeconds(0.4f);
        spriteRend.color = baseColor;
    }

    IEnumerator ResetKnockback() //stop leech from moving like it is being knocked back
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        knockedBack = false;
    }
}
