using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class LeechRoot : BTree
{
    [SerializeField] float idleVelo = 0.7f;
    [SerializeField] float velocity = 1.5f;
    [SerializeField] float maxAlertDistance = 30.0f;

    [SerializeField] float maxVelocity = 10.0f;
    [SerializeField] float minVelocity = 2.0f;

    //speed while in attack mode
    [SerializeField] float attackVelo = 10.0f;
    public float timeRemaining;

    public Vector3 offset;
    //distance leech attaches at
    float attachDistance = 1.9f;
    //drain while in attached mode
    [SerializeField] float attachedDrain = 1.5f;
    int drainDamage = 1;

    //leech pathfinding
    [SerializeField] PathGraph pfGraph;
    PathNode pfCurNode; //in the beginning this is set to the leech's starting node
    //[SerializeField] List<PathNode> pfPath = new List<PathNode>();
    [SerializeField] int pfRoom;
    PathNode pfEndNode { get; set; }
    int index { get; set; }

    BoxCollider2D lc;
    public Rigidbody2D rb { get; set; }

    [SerializeField] GameObject player;
    [SerializeField] GameObject leechTrans;
    [SerializeField] Rigidbody2D leechRb;

    Transform originalparent;

    protected override Node SetUpTree()
    {
        originalparent = leechTrans.transform.parent;

        Node root = new SelNode(new List<Node> { new SelNode(new List<Node> {
            new SeqNode(new List<Node>{ new IsAttached(player, leechTrans.transform, attachDistance),
                new DamagePlayer(player, leechTrans, attachedDrain, drainDamage)}),
            new SeqNode(new List<Node>{
                new inRadiusOfPlayer(leechTrans.transform, player, maxAlertDistance),
            new ChargePlayer(player, leechTrans, minVelocity, maxVelocity, originalparent, pfGraph, pfCurNode, pfEndNode, idleVelo) })}),
            new WanderToPlayer(pfGraph, pfCurNode, player, leechTrans, idleVelo, pfEndNode, index)});

        return root;
    }
}
