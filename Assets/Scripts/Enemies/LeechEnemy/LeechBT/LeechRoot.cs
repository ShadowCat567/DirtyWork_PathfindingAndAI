using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class LeechRoot : BTree
{
    [SerializeField] float idleVelo = 0.7f; //leech's idle velocity
    [SerializeField] float velocity = 1.5f; //leech's active velocity
    [SerializeField] float maxAlertDistance = 30.0f; //distance within which, leech is alerted to player's presence

    [SerializeField] float maxVelocity = 10.0f; //max velocity leech can travel at
    [SerializeField] float minVelocity = 2.0f; //min velocity leech can travel at

    float attachDistance = 1.2f; //distance leech attaches at
    [SerializeField] float attachedDrain = 1.5f; //amount of time between ticks of damage while in leech is attached
    int drainDamage = 1; //amount of damage leech does with each tick

    //leech pathfinding
    [SerializeField] PathGraph pfGraph; //referce to pathfinding graph
    PathNode pfCurNode; //at the start of the game, this is set to the leech's starting node
    [SerializeField] int pfRoom; //which room leech is in
    PathNode pfEndNode { get; set; } //node that marks the end of a path the leech generates
    int index { get; set; } //which node in the path is leech currently on

    [SerializeField] GameObject player; //reference tp player
    [SerializeField] GameObject leech; //reference to leech's game object

    Transform originalparent; //reference to the leech game object's original parent

    protected override Node SetUpTree() //sets up the leech's behavior tree
    {
        originalparent = leech.transform.parent;

        Node root = new SelNode(new List<Node> { new SelNode(new List<Node> {
            new SeqNode(new List<Node>{ new IsAttached(player, leech.transform, attachDistance),
                new DamagePlayer(player, leech, attachedDrain, drainDamage)}),
            new SeqNode(new List<Node>{
                new inRadiusOfPlayer(leech.transform, player, maxAlertDistance),
            new ChargePlayer(player, leech, minVelocity, maxVelocity, originalparent, pfGraph, pfCurNode, pfEndNode, idleVelo) })}),
            new WanderToPlayer(pfGraph, pfCurNode, player, leech, idleVelo, pfEndNode, index)});

        return root;
    }
    /* Structure of the Leech's Behavior tree: 
                                             Root(Selector)
                                         /                   \
                                  Selector                   WanderToPlayer
                           /                   \
                Seqential                         Seqential
              /          \                        /       \
       IsAttached     DamagePlayer     InRadiusOfPlayer    ChargePlayer
     */
}
