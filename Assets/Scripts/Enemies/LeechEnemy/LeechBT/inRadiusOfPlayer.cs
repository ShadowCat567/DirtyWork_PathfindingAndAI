using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class inRadiusOfPlayer : Node
{
    //tells if we are in range to start charging the player
    //failure if out of range
    GameObject _player;
    Transform _leechTrans;
    float _alertRadius;

    public inRadiusOfPlayer(Transform leech, GameObject player, float alertRadius)
    {
        _leechTrans = leech;
        _player = player;
        _alertRadius = alertRadius;
    }

    public override NodeState Evaluate()
    {
        //if player is within leech's alert radius, return success, otherwise return failure
        if (Vector2.Distance(_leechTrans.position, _player.transform.position) < _alertRadius)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
