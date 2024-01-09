using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehTree;

public class IsAttached : Node
{
    GameObject _player;
    Transform _leechTrans;
    float _attachRadius;

    public IsAttached(GameObject player, Transform leech, float attachRad)
    {
        _player = player;
        _leechTrans = leech;
        _attachRadius = attachRad;
    }

    public override NodeState Evaluate()
    {
        //if player is within attached radius, return success, otherwise return failure
        if (Vector2.Distance(_leechTrans.position, _player.transform.position) <= _attachRadius)
        {
            state = NodeState.SUCCESS;
        }
        else
        {
            state = NodeState.FAILURE;
        }

        return state;
    }
}
